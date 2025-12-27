using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using FMM.ViewModels;
using ReactiveUI;
using System.Linq;

namespace FMM.Views;

public partial class NationListView : UserControl
{
    public NationListView()
    {
        InitializeComponent();
    }

    private NationListViewModel? ViewModel => DataContext as NationListViewModel;

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

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        if (this.GetVisualRoot() is Window window)
        {
            window.Close();
        }
    }
}