using System.Diagnostics;
using System.Windows;

namespace FMMEditor.Views
{
    /// <summary>
    /// Interaction logic for ClubsView.xaml
    /// </summary>
    public partial class ClubsView : MahApps.Metro.Controls.MetroWindow
    {
        public ClubsView()
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
            this.Close();
        }
    }
}
