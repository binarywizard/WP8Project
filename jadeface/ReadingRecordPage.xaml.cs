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

        private BookListItem book;
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
                    ReadingRecordGrid.DataContext = book;
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
            ReadingRecordHistory.ItemsSource = records;
        }

        private void ConfirmRecord_Click(object sender, RoutedEventArgs e)
        {
            Button t = (Button)sender;
            ReadingRecord record = t.DataContext as ReadingRecord;

            int StartPageNo = 0;
            int EndPageNo = 0;
            if (int.TryParse(StartPage.Text, out StartPageNo) && int.TryParse(EndPage.Text, out EndPageNo))
            {
                if (StartPageNo <= EndPageNo && StartPageNo >= 0 && EndPageNo <= book.PageNo)
                {
                    record.StartPageNo = StartPageNo;
                    record.EndPageNo = EndPageNo;

                    record.ISBN = currentISBN;
                    record.UserId = phoneAppServeice.State["username"].ToString();
                    record.Timestamp = DateTime.Now.ToString();
                    bookService.insertRecord(record);

                    if (EndPageNo == book.PageNo)
                    {
                        book.Status = BookStatus.FINISHED;
                        bookService.update(book);
                        MessageBox.Show("又读完了一本书！");
                    }
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
            RefreshReadingRecord();
        }
    }
}