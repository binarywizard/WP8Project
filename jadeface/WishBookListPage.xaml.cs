﻿using System;
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
    public partial class WishBookListPage : PhoneApplicationPage
    {
        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;

        private BookService bookService;

        public WishBookListPage()
        {
            InitializeComponent();
        }

        private void DeleteBookListItem(BookListItem book)
        {
            bookService.delete(book, phoneAppServeice.State["username"].ToString());
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("[DEBUG]Navigate to WishBookListPage...");
            bookService = BookService.getInstance();
            RefreshWishBookList();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // dbConn.Close();
            Debug.WriteLine("[DEBUG]Leave WishBookListPage...");
        }


        private MobileServiceCollectionView<BookListItem> BookListRecheck()
        {
            //return bookListTable.ToCollectionView();
            return null;
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



        private void BookListItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认从书单中删除？", "确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Button tb = (Button)sender;
                BookListItem book = tb.DataContext as BookListItem;
                WishBookListItems.ItemsSource.Remove(book);
                //DeleteBookListItem(book);
                //booksTable.DeleteAsync(book);
                MessageBox.Show("删除完毕！");
                RefreshWishBookList();
            }

        }

        private void BookDetail_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            BookListItem book = b.DataContext as BookListItem;
            NavigationService.Navigate(new Uri("/BookDetailPage.xaml?BookISBN=" + book.ISBN, UriKind.Relative));
        }

        private void Reading_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            BookListItem book = b.DataContext as BookListItem;
            book.Status = BookStatus.READING;
            bookService.update(book);
            MessageBox.Show("已经将本书添加到正在阅读的列表中！");

            //RefreshWishBookList();


            RefreshWishBookList();
            //RefreshFinishBookList();
        }

        private void FinishRead_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            BookListItem book = b.DataContext as BookListItem;
            book.HaveReadPage = book.PageNo;
            book.Status = BookStatus.FINISHED;
            bookService.update(book);
            MessageBox.Show("又读完了一本书！");
            RefreshWishBookList();

            //RefreshFinishBookList();
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
            RefreshWishBookList();
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

        private void SelectABook(Object sender, SelectionChangedEventArgs e)
        {
            var books = (LongListSelector)sender;
            BookListItem book = books.SelectedItem as BookListItem;
            NavigationService.Navigate(new Uri("/ReadingRecordPage.xaml?BookISBN=" + book.ISBN, UriKind.Relative));
        }

        

        
    }
}