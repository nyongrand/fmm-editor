using MahApps.Metro.Controls;
using System.Windows;

namespace FMEViewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class NameView : MetroWindow
    {
        public NameView()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
