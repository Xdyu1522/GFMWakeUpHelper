using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GFMWakeUpHelper.Data.Entities;
using Serilog;

namespace GFMWakeUpHelper.App.Features.SongManageView;

public partial class SongManageView : UserControl
{
    public SongManageView()
    {
        InitializeComponent();

        this.AttachedToVisualTree += (_, _) =>
        {
            if (DataContext is SongManageViewModel vm)
            {
                vm.ClearSearch();
                vm.ClearCommands();
                vm.ReloadData();
            }
        };
    }


    private void TitleColumn_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb && tb.Tag is SongViewModel song && DataContext is SongManageViewModel vm)
        {
            string oldValue = song.Title;
            string newValue = !string.IsNullOrEmpty(tb.Text) ? tb.Text : string.Empty;
            if (oldValue != newValue)
            {
                vm.OnTitleChanged(song.Id, oldValue, newValue);
                song.Title = newValue;
                Log.Information("用户修改歌曲 {SongId} 的标题：{OldValue} → {NewValue}", song.Id, oldValue, newValue);
            }
        }
    }

    private void IsActiveChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb && cb.IsFocused && cb.Tag is SongViewModel song &&
            DataContext is SongManageViewModel vm)
        {
            bool newValue = cb.IsChecked ?? false;
            bool oldValue = !newValue; // 因为事件触发后状态已改变，旧值是反的

            if (oldValue != newValue)
            {
                vm.OnIsActiveChanged(song.Id, oldValue, newValue);
                Log.Information("用户修改歌曲 {SongId} 的可用状态：{OldValue} → {NewValue}", song.Id, oldValue, newValue);
            }
        }
    }
}