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
using Microsoft.Phone.Scheduler;

namespace jadeface
{
    public partial class EditReadingPlan : PhoneApplicationPage
    {

        private BookService bookService;
        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;
        //List<BookListItem> bl;

        ReadingPlan plan = null;

        bool flag = true;//标记是否第一次进入页面

        bool isRemind = false;//标记这个计划之前是否需要提醒

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
           

            plan = GetSeletcedPlan();

            

            bookService = BookService.getInstance();
            
            List<string> pl = new List<string>() { "高", "中", "低" };
            this.prioritylist.ItemsSource = pl;

            this.prioritylist.SelectedItem = plan.Priority;

            this.bookname.Text = plan.Title;
            //this.bookname.IsEnabled = false;
            this.bookname.IsReadOnly = true;
            this.bookname.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

            if (flag)
            {
                this.datePicker.Value = DateTime.Parse(plan.DatePicker);
                this.timepicker.Value = DateTime.Parse(plan.RingTime);
                flag = false;
            }
            

            this.detail.Text = plan.Detail;

            this.toggle.IsChecked = plan.IsReminder;

            isRemind = plan.IsReminder;

           
            
        }

        private ReadingPlan GetSeletcedPlan()
        {
            string planid;
            ReadingPlan selectedplan = null;
            if (NavigationContext.QueryString.TryGetValue("planID", out planid))
            {
                dbPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "jadeface.sqlite"));
                dbConn = new SQLiteConnection(dbPath);
                int Id = Int32.Parse(planid);
                SQLiteCommand command = dbConn.CreateCommand("select * from readingplan where Id = " + Id);
                List<ReadingPlan> plans = command.ExecuteQuery<ReadingPlan>();
                if (plans.Count == 1)
                {
                    selectedplan = plans.First();

                }
            }
            else
            {
                MessageBox.Show("编辑计划出错！");
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }

            return selectedplan;
        }

        private void ApplicationBar_StateChanged(object sender, Microsoft.Phone.Shell.ApplicationBarStateChangedEventArgs e)
        {
        }

        //void picker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        //{
        //    DateTime date = (DateTime)e.NewDateTime;
        //    //this.datePicker.Value = date;
        //    MessageBox.Show(date.ToString("d"));
        //}


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

                string clockname = "alarm" + plan.ISBN;
                //IEnumerable <Alarm> list = ScheduledActionService.GetActions<Alarm>();


                if (isRemind)
                {
                    if (ScheduledActionService.Find(clockname) != null)
                        ScheduledActionService.Remove(clockname);
                }
                Alarm clock = new Alarm(clockname);
                //开始时间(注意考虑开始时间小于系统时间的情况)
                DateTime beginTime = (DateTime)this.timepicker.Value;
                TimeSpan timespan = beginTime.TimeOfDay;
                if (beginTime < DateTime.Now)
                {
                    DateTime date = DateTime.Now.AddDays(1).Date;

                    beginTime = date + timespan;

                    Debug.WriteLine("[Debug]date:" + date + "timespan" + timespan + "beginTime.TimeOfDay" + beginTime);
                }
                clock.BeginTime = beginTime;
                //结束时间
                //clock.ExpirationTime = clock.BeginTime + new TimeSpan(0, 0, 30);

                DateTime expirationtime = (DateTime)this.datePicker.Value + timespan;
                Debug.WriteLine("[Debug]expirationtime:" + expirationtime);

                if (expirationtime < beginTime)
                {
                    MessageBox.Show("截止提醒时间已过，请修改截止时间或提醒时间");
                    return;
                }
                clock.ExpirationTime = expirationtime;

                //提醒内容
                clock.Content = "别忘了今天要读<<" + plan.Title + ">>.";


                //提醒铃声
                clock.Sound = new Uri("/SleepAway.mp3", UriKind.Relative);

                //提醒类型
                clock.RecurrenceType = RecurrenceInterval.Daily;

                ScheduledActionService.Add(clock);


            }
            else
            {
                plan.Image = "";

                string clockname = "alarm" + plan.ISBN;
                if (isRemind)
                {
                    if (ScheduledActionService.Find(clockname) != null)
                        ScheduledActionService.Remove(clockname);
                }
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