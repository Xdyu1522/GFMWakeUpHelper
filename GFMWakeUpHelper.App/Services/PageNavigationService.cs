using GFMWakeUpHelper.App.Features;
using System;

namespace GFMWakeUpHelper.App.Services;

public class PageNavigationService
{
    public Action<Type>? NavigationRequested { get; set; }

    public void RequestNavigation<T>() where T : PageBase
    {
        NavigationRequested?.Invoke(typeof(T));
    }
}