using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using GFMWakeUpHelper.App.Features;
using GFMWakeUpHelper.App.Services;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace GFMWakeUpHelper.App;

public class GFMWakeUpHelperMainViewModel : ObservableObject
{
    private readonly IEnumerable<PageBase> _pages;
    private PageBase _activePage;
    private readonly PageNavigationService _navigationService;
    private readonly ISukiDialogManager _dialogManager;

    public IEnumerable<PageBase> Pages => _pages.OrderBy(p => p.Index).ThenBy(p => p.DisplayName);

    public PageBase ActivePage
    {
        get => _activePage;
        set => SetProperty(ref _activePage, value);
    }

    public PageNavigationService NavigationService => _navigationService;
    public ISukiToastManager ToastManager { get; }
    public ISukiDialogManager DialogManager { get; }

    public GFMWakeUpHelperMainViewModel(IEnumerable<PageBase> pages, PageNavigationService navigationService,
        ISukiToastManager toastManager, ISukiDialogManager dialogManager)
    {
        _pages = pages;
        _navigationService = navigationService;
        ToastManager = toastManager;
        DialogManager = dialogManager;

        // 订阅导航请求
        _navigationService.NavigationRequested += pageType =>
        {
            var page = _pages.FirstOrDefault(p => p.GetType() == pageType);
            if (page != null && page != ActivePage)
            {
                ActivePage = page;
            }
        };

        // 设置默认页面
        if (_pages.Any())
        {
            ActivePage = _pages.OrderBy(p => p.Index).First();
        }
    }
}