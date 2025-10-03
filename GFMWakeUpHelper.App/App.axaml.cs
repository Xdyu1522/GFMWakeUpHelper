using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using GFMWakeUpHelper.App.Common;
using GFMWakeUpHelper.App.Dialogs.AskSameSongDialog;
using GFMWakeUpHelper.App.Features;
using GFMWakeUpHelper.App.Features.AddSongView;
using GFMWakeUpHelper.App.Features.SongManageView;
using GFMWakeUpHelper.App.Services;
using GFMWakeUpHelper.Data;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;

namespace GFMWakeUpHelper.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        using (var db = new DataDbContext())
        {
            db.Database.EnsureCreated();
        }
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            //desktop.MainWindow = new MainWindow
            //{
            //    DataContext = new MainWindowViewModel(),
            //};
            var services = new ServiceCollection();
            services.AddSingleton(desktop);
            var views = ConfigureViews(services);
            var provider = ConfigureServices(services);
            DataTemplates.Add(new ViewLocator(views));
            desktop.MainWindow = views.CreateView<GFMWakeUpHelperMainViewModel>(provider) as Window;

        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private static SukiViews ConfigureViews(ServiceCollection services)
    {
        // 然后注册视图和视图模型
        return new SukiViews()
            .AddView<GFMWakeUpHelperMainView, GFMWakeUpHelperMainViewModel>(services)
            .AddView<AddSongView, AddSongViewModel>(services)
            .AddView<SongManageView, SongManageViewModel>(services)
            .AddView<AskSameSongDialog, AskSameSongDialogViewModel>(services);
    }


    private static ServiceProvider ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<PageNavigationService>();
        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();

        // 所有服务注册已移至ConfigureViews方法中
        return services.BuildServiceProvider();
    }
    

}