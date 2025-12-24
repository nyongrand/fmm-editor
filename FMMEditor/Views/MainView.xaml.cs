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

        private void OpenCompetitions_Click(object sender, RoutedEventArgs e)
        {
            var competitionsView = new CompetitionsView();
            competitionsView.Show();
            this.Close();
        }

        private void OpenClubs_Click(object sender, RoutedEventArgs e)
        {
            var clubsView = new ClubsView();
            clubsView.Show();
            this.Close();
        }

        private void OpenNations_Click(object sender, RoutedEventArgs e)
        {
            var nationView = new NationsView();
            nationView.Show();
            this.Close();
        }

        private void OpenNames_Click(object sender, RoutedEventArgs e)
        {
            var namesView = new NamesView();
            namesView.Show();
            this.Close();
        }

        private void OpenPeople_Click(object sender, RoutedEventArgs e)
        {
            var peopleView = new PeopleView();
            peopleView.Show();
            this.Close();
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
