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
using System.ComponentModel;


namespace jadeface
{
    public partial class FinishBookListPage : PhoneApplicationPage
    {
        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;

        private BookService bookService;

        public FinishBookListPage()
        {
            InitializeComponent();
        }

        private void DeleteBookListItem(BookListItem book)
        {
            bookService.delete(book, phoneAppServeice.State["username"].ToString());
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("[DEBUG]Navigate to FinishBookListPage...");
            bookService = BookService.getInstance();
            RefreshFinishBookList();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // dbConn.Close();
            Debug.WriteLine("[DEBUG]Leave FinishBookListPage...");
        }


        private MobileServiceCollectionView<BookListItem> BookListRecheck()
        {
            //return bookListTable.ToCollectionView();
            return null;
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



        private void BookListItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认从书单中删除？", "确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Button tb = (Button)sender;
                BookListItem book = tb.DataContext as BookListItem;
                FinishBookListItems.ItemsSource.Remove(book);
                //DeleteBookListItem(book);
                //booksTable.DeleteAsync(book);
                MessageBox.Show("删除完毕！");
                RefreshFinishBookList();
            }

        }  

        private void Reading_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            BookListItem book = b.DataContext as BookListItem;
            book.Status = BookStatus.READING;
            bookService.update(book);
            MessageBox.Show("已经将本书添加到正在阅读的列表中！");

            //RefreshWishBookList();


            RefreshFinishBookList();
            //RefreshFinishBookList();
        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //获取选中的menuItem对象
            MenuItem menuItem = (MenuItem)sender;
            //获取对象的标题头的内容 
            string header = (sender as MenuItem).Header.ToString();
            //获取选中的LonglistSelectorItem
            BookListItem book = this.FinishBookListItems.SelectedItem as BookListItem;

            //如果没有选中则返回
            if (book == null)
            {
                return;
            }
            Debug.WriteLine("[DEBUG]Book is : " + book.Title);
            Debug.WriteLine("[DEBUG]Book Status is : " + book.Status);

            if (menuItem.Header.ToString() == "要重新读一下")
            {
                book.Status = BookStatus.READING;
                Debug.WriteLine("[DEBUG]Book Status is : " + book.Status);
                bookService.update(book);
                RefreshFinishBookList();
            }
            else if (menuItem.Header.ToString() == "有时间重新读一下")
            {
                book.Status = BookStatus.WISH;
                bookService.update(book);
                RefreshFinishBookList();
            }
            else if (menuItem.Header.ToString() == "删除")
            {
                //只删除书籍信息，不删除书籍相关的记录等
                bookService.delete(book, phoneAppServeice.State["username"].ToString());
                RefreshFinishBookList();
            }
        }

        private void SelectAFinishBook(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var books = (LongListSelector)sender;
            BookListItem book = books.SelectedItem as BookListItem;
            if (book == null)
            {
                return;
            }

            //(sender as LongListSelector).SelectedItem = null;

            NavigationService.Navigate(new Uri("/BookDetailPage.xaml?BookISBN=" + book.ISBN, UriKind.Relative));
        }




    }
}