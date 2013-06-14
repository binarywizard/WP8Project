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
using System.Windows.Media;

namespace jadeface
{
    public partial class EditReadingPlan : PhoneApplicationPage
    {

        private BookService bookService;
        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;
        List<BookListItem> bl;

        ReadingPlan plan = null;

        //Local Database Storage Definition
        /// <summary>
        /// The database path.
        /// </summary>
        private string dbPath;

        /// <summary>
        /// The sqlite connection.
        /// </summary>
        private SQLiteConnection dbConn;
        public EditReadingPlan()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.datePicker.ValueChanged += new EventHandler<DateTimeValueChangedEventArgs>(picker_ValueChanged);

            string planid;
            

            if (NavigationContext.QueryString.TryGetValue("planID", out planid))
            {
                dbPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "jadeface.sqlite"));
                dbConn = new SQLiteConnection(dbPath);
                int Id = Int32.Parse(planid);
                SQLiteCommand command = dbConn.CreateCommand("select * from readingplan where Id = " + Id);
                List<ReadingPlan> plans = command.ExecuteQuery<ReadingPlan>();
                if (plans.Count == 1)
                {
                    plan = plans.First();
                   
                }
            }
            else
            {
                MessageBox.Show("编辑计划出错！");
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }

            bookService = BookService.getInstance();
            
            List<string> pl = new List<string>() { "高", "中", "低" };
            this.prioritylist.ItemsSource = pl;
            
            this.prioritylist.SelectedItem = plan.Priority;

            this.bookname.Text = plan.Title ;
            //this.bookname.IsEnabled = false;
            this.bookname.IsReadOnly = true;
            this.bookname.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

            //this.datePicker.Value = Convert.ToDateTime(plan.DatePicker);
            //this.timepicker.Value = Convert.ToDateTime(plan.RingTime);

            this.detail.Text = plan.Detail;

            this.toggle.IsChecked = plan.IsReminder;

            //string username = phoneAppServeice.State["username"].ToString();
            //dbPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "jadeface.sqlite"));
            //dbConn = new SQLiteConnection(dbPath);
            //SQLiteCommand command = dbConn.CreateCommand("select * from booklistitem where userid = '" + username + "' and status = 1");
            //List<BookListItem> books = command.ExecuteQuery<BookListItem>();

            //Debug.WriteLine("[DEBUG]The count of book name list : " + books.Count);

            //List<string> bookns = new List<string>();
            //foreach (BookListItem book in books)
            //{
            //    bookns.Add(book.Title);
            //}
            //bl = books;
            //this.booknamelist.ItemsSource = bookns;
            
        }


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
  
            plan.DatePicker = this.datePicker.ValueString;
            plan.Priority = (string)this.prioritylist.SelectedItem;
            plan.IsReminder = (bool)this.toggle.IsChecked;
            plan.RingTime = this.timepicker.ValueString;
            plan.Detail = this.detail.Text.ToString();

            if (plan.IsReminder)
            {
                plan.Image = "/Icon/feature.alarm.png";
            }
            else
            {
                plan.Image = "";
            }

            Debug.WriteLine("[DEBUG]plan.userid: " + plan.UserId + "plan.ISBN;" + plan.ISBN + "plan.title:" + plan.Title +
                "plan.dataPicker:" + plan.DatePicker + "plan.priority;" + plan.Priority + "plan.IsReminder:" + plan.IsReminder + "plan.ringtime:" + plan.RingTime
                + "plan.Detail:" + plan.Detail);

            bool result = bookService.updateReadingPlan(plan);
            if (result)
            {
                //MessageBox.Show("succeed!");

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