using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows;

namespace FMMEditor.Views
{
    /// <summary>
    /// Interaction logic for NationView.xaml
    /// </summary>
    public partial class NationsView : MetroWindow
    {
        public NationsView()
        {
            InitializeComponent();
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://ko-fi.com/ngskicker",
                UseShellExecute = true
            });
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
