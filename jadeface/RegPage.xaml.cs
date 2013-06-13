using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.Phone.Net.NetworkInformation;
using System.Diagnostics;

namespace jadeface
{
    public partial class RegPage : PhoneApplicationPage
    {

        PhoneApplicationService phoneAppServeice = PhoneApplicationService.Current;

        private IMobileServiceTable<User> userTable = App.MobileService.GetTable<User>();

        public RegPage()
        {
            InitializeComponent();
        }

        private async void RegSubmitButtonClick(object sender, RoutedEventArgs e)
        {
            if (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None)
            {
                if (UserNameTextBox.Text != "" && PasswordTextBox.Password != "" && RePasswordTextBox.Password != "")
                {
                    if (!UserNameTextBox.Text.Trim().Equals("default"))
                    {
                        ProgressIndicator progress = new ProgressIndicator
                        {
                            IsVisible = true,
                            IsIndeterminate = true,
                            Text = "注册中..."
                        };
                        SystemTray.SetProgressIndicator(this, progress);

                        IEnumerable<User> list = await userTable.ReadAsync();

                        List<User> userList = list.ToList();

                        // Boolean isReg = false;

                        User newuser;

                        foreach (User user in userList)
                        {
                            if (user.UserId.Equals(UserNameTextBox.Text))
                            {
                                MessageBox.Show("此用户名已经有人使用了，请重新选择一个！");
                                return;
                            }
                        }

                        if (!PasswordTextBox.Password.Equals(RePasswordTextBox.Password))
                        {
                            MessageBox.Show("两次密码输入不符，请重新输入！");
                            return;
                        }

                        newuser = new User();

                        newuser.UserId = UserNameTextBox.Text.Trim();
                        newuser.Password = PasswordTextBox.Password.Trim();

                        await userTable.InsertAsync(newuser);

                        MessageBox.Show("注册完成，现在返回登录页面！");
                        NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("用户名不能为default，请重新选择一个用户名！");
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
                MessageBox.Show("注册账户需要网络连接，请开启手机的移动网络。");
            }
        }

        private void RegBackButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }
    }
}