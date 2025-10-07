using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
            }
        };
    }


    private void IsActiveComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Console.WriteLine("SelectionChanged");
    }
}