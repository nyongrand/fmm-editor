using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using FMM.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace FMM.Views;

public partial class NameListView : UserControl
{
    public NameListView()
    {
        InitializeComponent();
    }

    private NameListViewModel? ViewModel => DataContext as NameListViewModel;

    private async void Load_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider == null || ViewModel == null)
            return;

        var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = false,
            Title = "Select FM database folder"
        });

        var path = folder?.FirstOrDefault()?.Path.LocalPath;
        if (!string.IsNullOrWhiteSpace(path))
        {
            ViewModel.FolderPath = path;
        }
    }

    private async void SaveAs_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider == null || ViewModel == null)
            return;

        var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = false,
            Title = "Select target folder"
        });

        var path = folder?.FirstOrDefault()?.Path.LocalPath;
        if (!string.IsNullOrWhiteSpace(path))
        {
            ViewModel.SaveAsCommand.Execute(path);
        }
    }

    private void Donate_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://ko-fi.com/nyongrand",
                UseShellExecute = true
            });
        }
        catch
        {
        }
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        if (this.GetVisualRoot() is Window window)
        {
            window.Close();
        }
    }
}
