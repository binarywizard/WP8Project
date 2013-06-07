using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using SQLite;
using System.IO;
using Windows.Storage;

namespace jadeface
{
    public partial class LoginPage : PhoneApplicationPage
    {
        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;

        //Local Database Storage Definition
        /// <summary>
        /// The database path.
        /// </summary>
        private string dbPath;

        /// <summary>
        /// The sqlite connection.
        /// </summary>
        // private SQLiteConnection dbConn;

        public LoginPage()
        {
            InitializeComponent();
            dbPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "jadeface.sqlite"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Threading.Thread.Sleep(1500);
            base.OnNavigatedFrom(e);
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text != "" && PasswordTextBox.Password != "")
            {
                phoneAppServeice.State["username"] = UserNameTextBox.Text;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("请完整输入用户名以及密码！");
            }            
        }
    }
}