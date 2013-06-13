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
    public partial class BookDetailPage : PhoneApplicationPage
    {
        //Local Database Storage Definition
        /// <summary>
        /// The database path.
        /// </summary>
        private string dbPath;

        /// <summary>
        /// The sqlite connection.
        /// </summary>
        private SQLiteConnection dbConn;

        private BookListItem book;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string ISBN = "";
                                                                                                                                             
            if (NavigationContext.QueryString.TryGetValue("BookISBN", out ISBN))
            {
                dbPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "jadeface.sqlite"));
                dbConn = new SQLiteConnection(dbPath);
                SQLiteCommand command = dbConn.CreateCommand("select * from booklistitem where isbn = '" + ISBN + "'");
                List<BookListItem> books = command.ExecuteQuery<BookListItem>();
                if (books.Count == 1)
                {
                    book = books.First();
                    BookDetailGrid.DataContext = book;
                }
            }
            else
            {
                MessageBox.Show("详细信息页面加载出错！");
                NavigationService.Navigate(new Uri("/MainPage.xaml",UriKind.Relative));
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            dbConn.Close();
            Debug.WriteLine("[DEBUG]Leave BookDetailPage...");
        }

        public BookDetailPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}