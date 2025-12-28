using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using FMM.ViewModels;
using System.Diagnostics;
using System;
using System.Linq;

namespace FMM.Views;

public partial class ClubListView : UserControl
{
    public ClubListView()
    {
        InitializeComponent();
    }

    private ClubListViewModel? ViewModel => DataContext as ClubListViewModel;

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
                FileName = "https://ko-fi.com/ngskicker",
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

    private async void CopyUid_Click(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        var club = ViewModel?.SelectedClub;
        if (clipboard != null && club != null)
        {
            await clipboard.SetTextAsync(club.Uid.ToString());
            ViewModel!.StatusMessage = "UID copied to clipboard";
        }
    }

    private async void CopyUidHex_Click(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        var club = ViewModel?.SelectedClub;
        if (clipboard != null && club != null)
        {
            var hex = Convert.ToHexString(BitConverter.GetBytes(club.Uid));
            await clipboard.SetTextAsync(hex);
            ViewModel!.StatusMessage = $"UID hex copied: {hex}";
        }
    }
}
