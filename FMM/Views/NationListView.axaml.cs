using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using FMM.ViewModels;
using System;
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
            await ViewModel.LoadFromFolderAsync(path);
        }
    }

    private async void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            await ViewModel.SaveAsync();
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
            await ViewModel.SaveAsAsync(path);
        }
    }

    private async void CopyUid_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var uid = ViewModel?.SelectedNation?.Uid;
        if (topLevel?.Clipboard != null && uid.HasValue)
        {
            await topLevel.Clipboard.SetTextAsync(uid.Value.ToString());
        }
    }

    private async void CopyUidHex_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var uid = ViewModel?.SelectedNation?.Uid;
        if (topLevel?.Clipboard != null && uid.HasValue)
        {
            var bytes = BitConverter.GetBytes(uid.Value);
            var hex = BitConverter.ToString(bytes).Replace("-", string.Empty);
            await topLevel.Clipboard.SetTextAsync(hex);
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