using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using SukiUI.Controls;

namespace GFMWakeUpHelper.App.Common;

public class ShowDialog
{
    public static async Task ShowWindowDialogAsync(Control control)
    {
        var root = control.GetVisualRoot();
        if (root is not Window parentWindow || control.DataContext is not ComplexTypeViewModel childViewModel || childViewModel.Value is null)
        {
            return;
        }

        var window = new PropertyGridWindow()
        {
            DataContext = childViewModel.Value,
            Title = childViewModel.DisplayName,
        };

        await window.ShowDialog(parentWindow);
    }
}