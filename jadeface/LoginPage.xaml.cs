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
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.Phone.Net.NetworkInformation;

namespace jadeface
{
    public partial class LoginPage : PhoneApplicationPage
    {
        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;

        private IMobileServiceTable<User> userTable = App.MobileService.GetTable<User>();

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
            // dbPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "jadeface.sqlite"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            System.Threading.Thread.Sleep(1500);
            base.OnNavigatedFrom(e);
        }

        private async void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None)
            {

                if (UserNameTextBox.Text != "" && PasswordTextBox.Password != "")
                {
                    ProgressIndicator progress = new ProgressIndicator
                    {
                        IsVisible = true,
                        IsIndeterminate = true,
                        Text = "登陆中..."
                    };
                    SystemTray.SetProgressIndicator(this, progress);

                    IEnumerable<User> list = await userTable.ReadAsync();

                    List<User> userList = list.ToList();

                    Boolean isMatch = false;

                    foreach (User user in userList)
                    {
                        if (user.UserId.Equals(UserNameTextBox.Text))
                        {
                            if (user.Password.Equals(PasswordTextBox.Password))
                            {
                                phoneAppServeice.State["username"] = UserNameTextBox.Text;
                                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                                isMatch = true;
                            }
                        }
                    }

                    if (!isMatch)
                    {
                        MessageBox.Show("用户名或者密码错误，请您检查后重新输入！");
                    }
                }
                else
                {
                    MessageBox.Show("请完整输入用户名以及密码！");
                }
            }
            else
            {
                Debug.WriteLine("[DEBUG]Network Interface Available Status:" + NetworkInterface.GetIsNetworkAvailable());
                Debug.WriteLine("[DEBUG]Network Interface Type Status:" + NetworkInterface.NetworkInterfaceType);
                MessageBox.Show("登陆需要网络连接，请开启手机的移动网络。");
            }
        }

        private void RegButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RegPage.xaml", UriKind.Relative));    
        }

        private void DefaultLoginButtonClick(object sender, RoutedEventArgs e)
        {
            phoneAppServeice.State["username"] = "default";
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}