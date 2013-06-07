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
using Microsoft.Phone.Testing;

namespace jadeface
{
    public partial class TestPage : PhoneApplicationPage
    {
        public TestPage()
        {
            InitializeComponent();
            this.Content = UnitTestSystem.CreateTestPage();

        }

        /*
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CustomMessageBox box = new CustomMessageBox();
            box.Title = "Choose One Option";

            Rating rt = new Rating();
            rt.RatingItemCount = 6;
            

            RadioButton rb1 = new RadioButton();
            rb1.Content = "Option1";
            RadioButton rb2 = new RadioButton();
            rb2.Content = "Option2";
            RadioButton rb3 = new RadioButton();
            rb3.Content = "Option3";
            StackPanel rbl = new StackPanel();

            rbl.Children.Add(rt);
            rbl.Children.Add(rb1);
            rbl.Children.Add(rb2);
            rbl.Children.Add(rb3);
            box.Content = rbl;

            box.LeftButtonContent = "OK";
            box.RightButtonContent = "Cancel";

            box.Show();

           

            box.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        if ((bool)rb1.IsChecked)
                        {
                            Debug.WriteLine("Option 1 Checked.");
                        }
                        if ((bool)rb2.IsChecked)
                        {
                            Debug.WriteLine("Option 2 Checked.");
                        }
                        if ((bool)rb3.IsChecked)
                        {
                            Debug.WriteLine("Option 3 Checked.");
                        }
                        Debug.WriteLine("Rating Value is " + rt.Value);
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
         * * */
    }

}