using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SQLite;
using System.IO;
using Windows.Storage;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace jadeface
{
    public partial class ReadingRecordPage : PhoneApplicationPage
    {
        public ReadingRecordPage()
        {
            InitializeComponent();
        }

        //Local Database Storage Definition
        /// <summary>
        /// The database path.
        /// </summary>
        private string dbPath;

        /// <summary>
        /// The sqlite connection.
        /// </summary>
        private SQLiteConnection dbConn;

        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;
        private BookService bookService;

        private BookListItem book = new BookListItem();
        private ReadingRecord record = new ReadingRecord();
        private string currentISBN;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string ISBN = "";

            if (NavigationContext.QueryString.TryGetValue("BookISBN", out ISBN))
            {
                currentISBN = ISBN;
                dbPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "jadeface.sqlite"));
                dbConn = new SQLiteConnection(dbPath);
                SQLiteCommand command = dbConn.CreateCommand("select * from booklistitem where isbn = '" + ISBN + "'");
                List<BookListItem> books = command.ExecuteQuery<BookListItem>();
                if (books.Count == 1)
                {
                    book = books.First();
                    BookInformationGrid.DataContext = book;

                    StartPage.Text = "";
                    EndPage.Text = "";
                    ReadingRecordPanel.DataContext = record;
                }
                Debug.WriteLine("[DEBUG]Navigate to ReadingRecordPage...");
                bookService = BookService.getInstance();
                RefreshReadingRecord();
            }
            else
            {
                MessageBox.Show("读书记录页面加载出错！");
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }

        }

        private void RefreshReadingRecord()
        {
            List<ReadingRecord> records = bookService.RefreshReadingRecord(phoneAppServeice.State["username"].ToString(), currentISBN);
            foreach (ReadingRecord record in records)
            {
                Debug.WriteLine("[DEBUG]Record belongs to the book with ISBN: " + record.ISBN);
            }
            int totalHaveReadPage = CaculateHaveReadPage(records);
            if (totalHaveReadPage == book.PageNo)
            {
                book.Status = BookStatus.FINISHED;
                MessageBox.Show("又读完了一本书！");
            }
            book.HaveReadPage = totalHaveReadPage;
            bookService.update(book);

            //int days2forecast = ForecastDays2Finish(records, totalHaveReadPage);
            Days2FinishTextBlock.Text = ForecastDays2Finish(records);

            readingProgressBar.DataContext = book;
            readingProgressBar.Value = book.HaveReadPage;

            ProgressTextBlock.DataContext = book;
            ProgressTextBlock.Text = ((double)book.HaveReadPage / book.PageNo * 100).ToString("f0") + "%";
            ReadingRecordHistory.ItemsSource = records;
        }

        private string ForecastDays2Finish(List<ReadingRecord> records)
        {
            if (records.Count == 0)
            {
                return "\u221E";
            }

            ReadingRecord record = records.First();

            TimeSpan ts = DateTime.Now - DateTime.Parse(record.Timestamp);

            int days = (ts.Days + 1) * (book.PageNo - book.HaveReadPage) / book.HaveReadPage + 1;
            return days.ToString();


        }

        private void ConfirmRecord_Click(object sender, RoutedEventArgs e)
        {
            Button t = (Button)sender;
            record = t.DataContext as ReadingRecord;

            int StartPageNo;
            int EndPageNo;
            if (int.TryParse(StartPage.Text, out StartPageNo) && int.TryParse(EndPage.Text, out EndPageNo))
            {
                Debug.WriteLine("[DEBUG]SartPageNo is : " + StartPageNo);
                Debug.WriteLine("[DEBUG]EndPageNo is : " + EndPageNo);
                if (StartPageNo <= EndPageNo && StartPageNo > 0 && EndPageNo <= book.PageNo)
                {
                    record.StartPageNo = StartPageNo;
                    record.EndPageNo = EndPageNo;

                    record.ISBN = currentISBN;
                    record.UserId = phoneAppServeice.State["username"].ToString();
                    record.Timestamp = DateTime.Now.ToString();
                    bookService.insertRecord(record);
                    record = null;

                    //if (EndPageNo == book.PageNo)
                    //{
                    //    book.Status = BookStatus.FINISHED;
                    //    bookService.update(book);
                    //    MessageBox.Show("又读完了一本书！");
                    //}
                }
                else if (StartPageNo > EndPageNo)
                {
                    MessageBox.Show("书读反了！请重新输入！");
                }
                else
                {
                    MessageBox.Show("页码的范围有误，请重新输入！");
                }

            }
            else
            {
                MessageBox.Show("请输入有效的页码！");
            }
            StartPage.Text = "";
            EndPage.Text = "";
            RefreshReadingRecord();
        }

        private List<Point> GetHaveReadPage(List<ReadingRecord> records)
        {
            List<Point> pageNoPairs = new List<Point>();
            foreach (var item in records)
            {
                Point point = new Point();
                point.X = item.StartPageNo;
                point.Y = item.EndPageNo;
                pageNoPairs.Add(point);
            }
            Debug.WriteLine("[DEBUG]pageNoPairs.Count = " + pageNoPairs.Count);
            foreach (var item in pageNoPairs)
            {
                Debug.WriteLine("[DEBUG]pageNoPair : " + item.X + " " + item.Y);
            }
            return pageNoPairs;
        }

        private List<Point> CombineHaveReadPage(List<Point> pageNoPairs)
        {
            List<Point> data = pageNoPairs;
            for (int i = 1; i < data.Count; i++)
            {
                for (int m = i; m < data.Count; m++)
                {
                    if (data[m].X <= data[i - 1].Y && data[m].Y >= data[i - 1].X && m != i - 1)
                    {
                        Point tempPoint = new Point();
                        tempPoint.X = data[i - 1].X > data[m].X ? data[m].X : data[i - 1].X;
                        tempPoint.Y = data[i - 1].Y > data[m].Y ? data[i - 1].Y : data[m].Y;
                        data[i - 1] = tempPoint;//结构体是值传递，要修改只能通过这种方式  
                        data.Remove(data[m]);
                        m = 0;//从头开始此次循环合并  
                    }
                }
            }

            foreach (var item in data)
            {
                Debug.WriteLine("[DEBUG]startPage and endPage : " + item.ToString());
            }

            return data;
        }

        private int CaculateHaveReadPage(List<ReadingRecord> records)
        {
            List<Point> pageNoPairs = GetHaveReadPage(records);
            List<Point> data = CombineHaveReadPage(pageNoPairs);

            int totalHaveReadPage = 0;
            foreach (var item in data)
            {
                totalHaveReadPage += (item.Y - item.X + 1);
            }
            return totalHaveReadPage;
        }

        private void FinishRead_Click(object sender, RoutedEventArgs e)
        {
            BookListItem book = readingProgressBar.DataContext as BookListItem;
            book.HaveReadPage = book.PageNo;
            book.Status = BookStatus.FINISHED;
            bookService.update(book);
            MessageBox.Show("又读完了一本书！");
            //RefreshWishBookList();
            //RefreshBookList();
            //RefreshFinishBookList();
        }


        private void ApplicationBarIconButton_Click_Check(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BookDetailPage.xaml?BookISBN=" + currentISBN, UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_Note(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NotesPage.xaml?BookISBN=" + currentISBN, UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_Refresh(object sender, EventArgs e)
        {
            RefreshReadingRecord();
        }

        private void RecordHistoryDrag(object sender, GestureEventArgs e)
        {
            this.ApplicationBar.IsVisible = false;
        }

        private void RecordHistoryDragCompleted(object sender, GestureEventArgs e)
        {
            this.ApplicationBar.IsVisible = true;
        }
    }

    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public override string ToString()
        {
            return string.Format("X = {0}, Y = {1}.", X, Y);
        }
    }
}