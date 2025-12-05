using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;

namespace FMMEditor.Views
{
    /// <summary>
    /// Interaction logic for PeopleView.xaml
    /// </summary>
    public partial class PeopleView : MetroWindow
    {
        public PeopleView()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://ko-fi.com/nyongrand",
                    UseShellExecute = true
                });
            }
            catch (Exception) { }
        }
    }
}
