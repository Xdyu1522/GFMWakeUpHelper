using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GFMWakeUpHelper.Data.Entities;
using SukiUI.Dialogs;

namespace GFMWakeUpHelper.App.Dialogs.AskSameSongDialog;

public partial class AskSameSongDialogViewModel : ObservableObject
{
    public ObservableCollection<Song> PendingSongs { get; set; }

    [ObservableProperty] private string _dialogTitle = "这是一样的音乐吗？";

    public AskSameSongDialogViewModel(IEnumerable<Song> data, string? title = null)
    {
        PendingSongs = new(data);

        if (!string.IsNullOrEmpty(title))
            DialogTitle = title;
    }
}