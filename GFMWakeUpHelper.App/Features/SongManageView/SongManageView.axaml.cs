using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GFMWakeUpHelper.Data.Entities;

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
                vm.ReloadData();
                vm.ClearCommands();
            }
        };
    }


    private void TitleColumn_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb && tb.Tag is Song song && DataContext is SongManageViewModel vm)
        {
            string oldValue = song.Title;
            string newValue = !string.IsNullOrEmpty(tb.Text) ? tb.Text : string.Empty;
            if (oldValue != newValue)
            {
                vm.OnTitleChanged(song.Id, oldValue, newValue);
                song.Title = newValue;
                Console.WriteLine($"{song.Id} - 标题修改: {oldValue} → {newValue}");
            }
        }
    }

    private void IsActiveChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb && cb.IsFocused && cb.Tag is Song song && DataContext is SongManageViewModel vm)
        {
            bool newValue = cb.IsChecked ?? false;
            bool oldValue = !newValue; // 因为事件触发后状态已改变，旧值是反的

            if (oldValue != newValue)
            {
                vm.OnIsActiveChanged(song.Id, oldValue, newValue);
                Console.WriteLine($"{song.Id} - 是否可用修改: {oldValue} → {newValue}");
            }
        }
    }
}