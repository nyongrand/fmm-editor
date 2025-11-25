using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;

namespace FMMEditor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DataGrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.Header = $"{e.Row.GetIndex() + 1}.";
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
