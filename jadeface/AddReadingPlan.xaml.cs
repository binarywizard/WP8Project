using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Diagnostics;
using SQLite;
using System.IO;
using Windows.Storage;

namespace jadeface
{
    public partial class AddReadingPlan : PhoneApplicationPage
    {
        private BookService bookService;
        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;
        List<BookListItem> bl;

        //Local Database Storage Definition
        /// <summary>
        /// The database path.
        /// </summary>
        private string dbPath;

        /// <summary>
        /// The sqlite connection.
        /// </summary>
        private SQLiteConnection dbConn;

        public AddReadingPlan()
        {
            InitializeComponent();
            //this.datePicker.ValueChanged += new EventHandler<DateTimeValueChangedEventArgs>(picker_ValueChanged);
            //List<string> pl = new List<string>() {"高","中","低" };
            //this.prioritylist.ItemsSource = pl;
            //this.prioritylist.SelectedItem = pl[1];

            //List<string> bookns = new List<string>();
            //List<BookListItem> bl = booklist();
            //foreach (var item in bl)
            //{
            //    bookns.Add(item.Title);
            //}
            //this.booknamelist.ItemsSource = bookns;


        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bookService = BookService.getInstance();
            this.datePicker.ValueChanged += new EventHandler<DateTimeValueChangedEventArgs>(picker_ValueChanged);
            List<string> pl = new List<string>() { "高", "中", "低" };
            this.prioritylist.ItemsSource = pl;
            this.prioritylist.SelectedItem = pl[1];

            string username = phoneAppServeice.State["username"].ToString();
            dbPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "jadeface.sqlite"));
            dbConn = new SQLiteConnection(dbPath);
            SQLiteCommand command = dbConn.CreateCommand("select * from booklistitem where userid = '" + username + "' and status = 1");
            List<BookListItem> books = command.ExecuteQuery<BookListItem>();

            Debug.WriteLine("[DEBUG]The count of book name list : " + books.Count);

            List<string> bookns = new List<string>();
            foreach (BookListItem book in books)
            {
                bookns.Add(book.Title);
            }
            bl = books;
            this.booknamelist.ItemsSource = bookns;
            base.OnNavigatedTo(e);
        }

        //private List<BookListItem> booklist()
        //{
        //    bookService = BookService.getInstance();
        //    List<BookListItem> books = bookService.RefreshWishBookList(phoneAppServeice.State["username"].ToString());
        //    foreach (BookListItem item in books)
        //    {
        //        Debug.WriteLine("[DEBUG]Item Status is : " + item.Status);
        //    }

        //    return books;

        //}

        private void ApplicationBar_StateChanged(object sender, Microsoft.Phone.Shell.ApplicationBarStateChangedEventArgs e)
        {
        }

        void picker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime date = (DateTime)e.NewDateTime;
            MessageBox.Show(date.ToString("d"));
        }

        

        private void toggle_Click(object sender, RoutedEventArgs e)
        {
            if (this.toggle.IsChecked == true)
            {
                this.toggle.Content = "开";
            }
            else if (this.toggle.IsChecked == false)
            {
                this.toggle.Content = "关";
            }
           
        }

        //private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (this.bookname.Text.Equals("书名"))
        //    {
        //        this.bookname.Text = "";
        //        this.bookname.Foreground = new SolidColorBrush(Color.FromArgb(255,0,0,0));
        //    }
        //}


       
        private void save_clicked(object sender, EventArgs e)
        {
            ReadingPlan plan = new ReadingPlan();
            plan.UserId = phoneAppServeice.State["username"].ToString();
            plan.ISBN = bl[booknamelist.SelectedIndex].ISBN;
           
            plan.Title = (string)this.booknamelist.SelectedItem;
            plan.DatePicker = this.datePicker.ValueString;
            plan.Priority = (string)this.prioritylist.SelectedItem;
            plan.IsReminder = (bool)this.toggle.IsChecked;
            plan.RingTime = this.timepicker.Value.ToString();
            plan.Detail = this.detail.Text.ToString();

            if (plan.IsReminder)
            {
                plan.Image = "/Icon/111.png";
            }
            else
            {
                plan.Image = "/Icon/222.png";
            }

            Debug.WriteLine("[DEBUG]plan.userid: " + plan.UserId + "plan.ISBN;" + plan.ISBN +"plan.title:"+plan.Title+
                "plan.dataPicker:"+plan.DatePicker+"plan.priority;"+plan.Priority+"plan.IsReminder:"+plan.IsReminder+"plan.ringtime:"+plan.RingTime
                +"plan.Detail:"+plan.Detail);

            bool result = bookService.insertPlan(plan);
            if (result)
            {
                MessageBox.Show("succeed!");
                
                //NavigationService.Navigate(new Uri("/MainPage.xaml",UriKind.Relative));
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("failed!");
            }

        }

        private void cancel_clicked(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
         
      
    }
}