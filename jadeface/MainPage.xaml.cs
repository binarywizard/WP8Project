using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using jadeface.Resources;

using Microsoft.WindowsAzure.MobileServices;
using System.IO;
using SQLite;
using Windows.Storage;
using Microsoft.Phone.Net.NetworkInformation;
using System.Diagnostics;

namespace jadeface
{

    public partial class MainPage : PhoneApplicationPage
    {
        // MobileServiceCollectionView implements ICollectionView (useful for databinding to lists) and 
        // is integrated with your Mobile Service to make it easy to bind your data to the ListView
        //private MobileServiceCollectionView<Book> books;
        //private MobileServiceCollectionView<BookListItem> bookList;
        //private MobileServiceCollectionView<BookListItem> bookRecheckList;

        //private IMobileServiceTable<Book> booksTable = App.MobileService.GetTable<Book>();
        //private IMobileServiceTable<BookListItem> bookListTable = App.MobileService.GetTable<BookListItem>();

        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;

        private BookService bookService;

        delegate void DownDelegate(List<BookListItem> books);
        DownDelegate downDelegate;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void DeleteBookListItem(BookListItem book)
        {
            bookService.delete(book, phoneAppServeice.State["username"].ToString());
        }


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            /*
            var todoItem = new TodoItem { Text = TodoInput.Text };
            InsertTodoItem(todoItem);

            var book = new Books { Title= "Test Book", PageNo = 233, Publisher = "战争图书馆" };
            InsertBooks(book);
            */
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("[DEBUG]Navigate to MainPage...");
            bookService = BookService.getInstance();
            RefreshBookList();
            RefreshWishBookList();
            RefreshFinishBookList(); 
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // dbConn.Close();
            Debug.WriteLine("[DEBUG]Leave MainPage...");
        }
        

        private MobileServiceCollectionView<BookListItem> BookListRecheck()
        {
            //return bookListTable.ToCollectionView();
            return null;
        }

        private void setConent(List<BookListItem> books)
        {
            SearchListItems.ItemsSource = books;
        }

        private void ResponseCallback(IAsyncResult result)  //async callback
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            WebResponse response = request.EndGetResponse(result);
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string contents = reader.ReadToEnd();
                    List<BookListItem> books = BookDataParser.parse(contents);
                    Dispatcher.BeginInvoke(downDelegate, books);//update ui via dispatcher
                }
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None)
            {
                if (BookToSearch.Text.Equals("") || BookToSearch.Text == "" || BookToSearch.Text == null)
                {
                    MessageBox.Show("请输入需要搜索的书名或者关键字！");
                }
                else
                {
                    string BookTitle = BookToSearch.Text;
                    string url = @"http://api.douban.com/v2/book/search?q=" + @BookTitle; //get book info from douban.com
                    downDelegate = setConent;
                    System.Net.WebRequest request = HttpWebRequest.Create(url);
                    IAsyncResult result = (IAsyncResult)request.BeginGetResponse(ResponseCallback, request); //async web request
                }
            }
            else
            {
                Debug.WriteLine("[DEBUG]Network Interface Available Status:" + NetworkInterface.GetIsNetworkAvailable());
                Debug.WriteLine("[DEBUG]Network Interface Type Status:" + NetworkInterface.NetworkInterfaceType);
                MessageBox.Show("搜索书籍信息需要网络连接，请开启手机的移动网络。");
            }
           
           
        }

        private void SearchItemClick(object sender, RoutedEventArgs e)
        {

            CustomMessageBox box = new CustomMessageBox();
            box.Title = "添加到...";

            RadioButton rb1 = new RadioButton();
            rb1.Content = "想读的书";
            RadioButton rb2 = new RadioButton();
            rb2.Content = "在读的书";
            RadioButton rb3 = new RadioButton();
            rb3.Content = "读完的书";
            StackPanel rbl = new StackPanel();
            rbl.Children.Add(rb1);
            rbl.Children.Add(rb2);
            rbl.Children.Add(rb3);
            box.Content = rbl;

            box.LeftButtonContent = "确定";
            box.RightButtonContent = "取消";

            box.Show();

            box.Dismissed += (s1, e1) =>
            {
                Button tb = (Button)sender;
                BookListItem book = tb.DataContext as BookListItem;
                book.UserId = phoneAppServeice.State["username"].ToString();
                book.Rating = 3;
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        if ((bool)rb1.IsChecked)
                        {
                            Debug.WriteLine("[DEBUG]A Wish Book Added.");
                            book.Status = BookStatus.WISH;
                        }
                        if ((bool)rb2.IsChecked)
                        {
                            Debug.WriteLine("[DEBUG]A Reading Book Added.");
                            book.Status = BookStatus.READING;
                        }
                        if ((bool)rb3.IsChecked)
                        {
                            Debug.WriteLine("[DEBUG]A Finish Book Added.");
                            book.CurPageNo = book.PageNo;
                            book.Status = BookStatus.FINISHED;
                        }
                        List<BookListItem> items = bookService.searchByISBN(book.ISBN, phoneAppServeice.State["username"].ToString());

                        if (items.Count == 0)
                        {
                            bookService.insert(book);
                            //InsertBookListItem(book);
                            RefreshWishBookList();
                            RefreshBookList();
                            RefreshFinishBookList();
                        }
                        else
                        {
                            MessageBox.Show("你已经添加过这本书，无需再次添加！");
                            return;
                        }
                        MessageBox.Show("添加完毕！");
                        break;
                    case CustomMessageBoxResult.RightButton:
                        break;
                    case CustomMessageBoxResult.None:
                        break;
                    default:
                        break;
                }
            };
        }

        private void RefreshBookList()
        {
            List<BookListItem> books = bookService.RefreshBookList(phoneAppServeice.State["username"].ToString());
            foreach (BookListItem item in books)
            {
                Debug.WriteLine("[DEBUG]Item Status is : " + item.Status);
            }
            BookListItems.ItemsSource = books;
        }

        private void RefreshWishBookList()
        {
            List<BookListItem> books = bookService.RefreshWishBookList(phoneAppServeice.State["username"].ToString());
            foreach (BookListItem item in books)
            {
                Debug.WriteLine("[DEBUG]Item Status is : " + item.Status);
            }
            WishBookListItems.ItemsSource = books;
        }

        private void RefreshFinishBookList()
        {
            List<BookListItem> books = bookService.RefreshFinishBookList(phoneAppServeice.State["username"].ToString());
            foreach (BookListItem item in books)
            {
                Debug.WriteLine("[DEBUG]Item Status is : " + item.Status);
            }
            FinishBookListItems.ItemsSource = books;
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            //books = booksTable.ToCollectionView();
            RefreshBookList();
        }

        private void BookListItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认从书单中删除？", "确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Button tb = (Button)sender;
                BookListItem book = tb.DataContext as BookListItem;
                BookListItems.ItemsSource.Remove(book);
                DeleteBookListItem(book);
                //booksTable.DeleteAsync(book);
                MessageBox.Show("删除完毕！");
                RefreshBookList();
            }
            
        }

        private void BookDetail_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            BookListItem book = b.DataContext as BookListItem;
            NavigationService.Navigate(new Uri("/BookDetailPage.xaml?BookISBN=" + book.ISBN, UriKind.Relative));
        }

        private void CurPageSave_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            BookListItem book = t.DataContext as BookListItem;
            
            int CurPageNo = 0;
            if (int.TryParse(t.Text, out CurPageNo))
            {
                if (CurPageNo < book.PageNo && CurPageNo >= 0)
                {
                    book.CurPageNo = CurPageNo;
                    bookService.update(book);
                }
                else if (CurPageNo == book.PageNo)
                {
                    book.CurPageNo = CurPageNo;
                    book.Status = BookStatus.FINISHED;
                    bookService.update(book);
                    MessageBox.Show("又读完了一本书！");
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
            RefreshBookList();
           
        }

        private void Reading_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            BookListItem book = b.DataContext as BookListItem;
            book.Status = BookStatus.READING;
            bookService.update(book);
            MessageBox.Show("已经将本书添加到正在阅读的列表中！");
            RefreshWishBookList();
            RefreshBookList();
            RefreshFinishBookList();
        }

        private void FinishRead_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            BookListItem book = b.DataContext as BookListItem;
            book.CurPageNo = book.PageNo;
            book.Status = BookStatus.FINISHED;
            bookService.update(book);
            MessageBox.Show("又读完了一本书！");
            RefreshWishBookList();
            RefreshBookList();
            RefreshFinishBookList();
        }

        private void WishButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshWishBookList();
        }

        private void FinishButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshFinishBookList();
        }

        private void BookRatingSave(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            BookListItem book = bt.DataContext as BookListItem;
            //book.Rating = ;
            Debug.WriteLine("[DEBUG]Rating is " + book.Rating);
            bookService.update(book);
            MessageBox.Show("本书的评分已经更新。");
            RefreshWishBookList();
            RefreshBookList();
            RefreshFinishBookList();
        }

        /*
         * 处理菜单栏点击逻辑
         */
        private void ApplicationBarIconButton_Click_Change(object sender, EventArgs e)
        {
        }

        private void ApplicationBarIconButton_Click_Add(object sender, EventArgs e)
        {
        }

        private void ApplicationBarIconButton_Click_Search(object sender, EventArgs e)
        {
        }

        private void ApplicationBarIconButton_Click_Refresh(object sender, EventArgs e)
        {
            List<BookListItem> books = bookService.RefreshBookList(phoneAppServeice.State["username"].ToString());
            foreach (BookListItem item in books)
            {
                Debug.WriteLine("[DEBUG]Item Status is : " + item.Status);
            }
            BookListItems.ItemsSource = books;
        }

        private void ApplicationBarMenuItem_Click_Setting(object sender, EventArgs e)
        {
        }

        private void ApplicationBarMenuItem_Click_Update(object sender, EventArgs e)
        {
        }

        private void ApplicationBarMenuItem_Click_About(object sender, EventArgs e)
        {
        }

        private void ApplicationBar_StateChanged(object sender, Microsoft.Phone.Shell.ApplicationBarStateChangedEventArgs e)
        {
        }

    }
}