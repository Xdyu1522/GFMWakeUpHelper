using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SukiUI.Controls;
using SukiUI.Toasts;

namespace GFMWakeUpHelper.App;

public partial class GFMWakeUpHelperMainView : SukiWindow
{
    public GFMWakeUpHelperMainView()
    {
        InitializeComponent();
        Width = 1200;
        Height = 700;
    }
}