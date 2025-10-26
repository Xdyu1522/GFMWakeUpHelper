using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Rendering;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GFMWakeUpHelper.App.Commands;
using GFMWakeUpHelper.App.Common;
using GFMWakeUpHelper.App.Converters;
using GFMWakeUpHelper.Data;
using GFMWakeUpHelper.Data.Entities;
using ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using Material.Icons;
using Microsoft.EntityFrameworkCore;
using ReactiveUI.Avalonia;
using Serilog;
using SukiUI.Toasts;

namespace GFMWakeUpHelper.App.Features.SongManageView;

public partial class SongManageViewModel : PageBase
{
    private readonly DataDbContext _dbContext = new();
    private readonly CommandsManager _commandsManager = new();
    private ISukiToastManager _toastManager;
    private readonly SourceList<SongViewModel> _sourceSongs = new();

    [ObservableProperty] private List<string> _searchFields = ["Id", "标题", "歌手", "添加批次"];
    [ObservableProperty] private string _selectedField = "Id";
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ObservableCollectionExtended<SongViewModel> _songs = [];
    [ObservableProperty] private int _recordCount;
    [ObservableProperty] private SongViewModel? _selectedSong;

    public SongManageViewModel(ISukiToastManager toastManager) : base("歌曲条目管理", MaterialIconKind.AboutCircleOutline,
        -150)
    {
        _toastManager = toastManager;
        var searchFilter = this.WhenAnyValue(x => x.SearchText, x => x.SelectedField)
            .ObserveOn(AvaloniaScheduler.Instance)
            .DistinctUntilChanged()
            .Select(_ => MakeFilter(SearchText))
            .StartWith(MakeFilter(""));
        var filtered = _sourceSongs.Connect()
            .ObserveOn(AvaloniaScheduler.Instance)
            .Filter(searchFilter)
            .Sort(SortExpressionComparer<SongViewModel>.Ascending(s => s.Id))
            .Bind(Songs)
            .Do(_ => RecordCount = Songs.Count)
            .DisposeMany()
            .Subscribe();
        ReloadData();
    }

    private Func<SongViewModel, bool> MakeFilter(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return _ => true;

        searchText = searchText.Trim().ToLowerInvariant();

        return SelectedField switch
        {
            "Id" => s => s.Id.ToString().Contains(searchText),
            "标题" => s => s.Title.ToLowerInvariant().Contains(searchText),
            "歌手" => s => s.Artists.Any(a => a.ToLowerInvariant().Contains(searchText)),
            "添加批次" => s => s.Batch.ToString().Contains(searchText),
            _ => _ => true
        };
    }

    public void ReloadData()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            try
            {
                _sourceSongs.Edit(updater =>
                {
                    updater.Clear();

                    var entities = _dbContext.Songs.AsNoTracking().ToList();
                    if (entities.Count > 0)
                    {
                        var viewModels = entities.Select(x => new SongViewModel(SongCloner.Clone(x))).ToList();
                        updater.AddRange(viewModels);
                    }
                });

                RecordCount = Songs.Count;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ReloadData 发生异常");
            }
        });
    }

    public void OnTitleChanged(int songId, string oldValue, string newValue)
    {
        _sourceSongs.Items.Single(s => s.Id == songId).Title = newValue;
        var builder = _commandsManager.GetCommandBuilder<EditTitleCommand>();
        var cmd = builder.ForSong(songId)
            .Set(s => s.OldValue, oldValue)
            .Set(s => s.NewValue, newValue)
            .Build();
        _commandsManager.Register(cmd);
    }


    partial void OnSelectedFieldChanged(string value)
    {
    }


    public void OnIsActiveChanged(int songId, bool oldValue, bool newValue)
    {
        _sourceSongs.Items.Single(s => s.Id == songId).IsActive = newValue;
        var builder = _commandsManager.GetCommandBuilder<EditAvailabilityCommand>();
        var cmd = builder.ForSong(songId)
            .Set(s => s.OldValue, oldValue)
            .Set(s => s.NewValue, newValue)
            .Build();
        _commandsManager.Register(cmd);
    }

    public void ClearCommands()
    {
        _commandsManager.Clear();
    }

    /*
    [RelayCommand]
    private async Task OnIsActiveChanged(SongParam<bool> parameters)
    {
        if (parameters.Song is null || parameters.NewValue == null)
            return;
        var currentCommand = await new SetAvailabilityCommandBuilder(_dbContext)
            .WithSongId(parameters.Song.Id)
            .SetTo(parameters.NewValue)
            .Build();
        Commands.Add(currentCommand);
    }*/
    public void ClearSearch() => SearchText = string.Empty;


    [RelayCommand]
    private void Refresh()
    {
        SearchText = string.Empty;
        _commandsManager.Clear();
        ReloadData();
    }

    [RelayCommand]
    private async Task SubmitChanges()
    {
        await _commandsManager.ExecuteAll(_dbContext);
        await _dbContext.SaveChangesAsync();
        Log.Information("所有更改已经保存");
    }

    [RelayCommand]
    private void CheckDuplicate()
    {
    }


    [RelayCommand]
    private void OnCheckDuplicates()
    {
        // TODO: 重复歌曲检测逻辑
    }

    [RelayCommand]
    private void DeleteSong()
    {
        if (SelectedSong != null)
        {
            var builder = _commandsManager.GetCommandBuilder<DeleteSongCommand>();
            var cmd = builder.ForSong(SelectedSong.Id)
                .Set(s => s.OldData, SelectedSong.ToSong())
                .Build();
            _commandsManager.Register(cmd);
            _sourceSongs.Remove(SelectedSong);
        }
        else
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("操作失败")
                .WithContent("请先选中音乐再进行删除")
                .Queue();
        }
    }

    [RelayCommand]
    private void Undo()
    {
        var undoCmd = _commandsManager.Undo();
        SongViewModel currentSong;
        switch (undoCmd)
        {
            case EditTitleCommand editTitle:
                currentSong = _sourceSongs.Items.Single(s => s.Id == editTitle.SongId);
                currentSong.Title = editTitle.OldValue;
                /*
                Songs.ChangeAndRefresh(currentSong.Id, s => s.Title, editTitle.OldValue);
                */
                break;
            case EditAvailabilityCommand editAvailability:
                currentSong = _sourceSongs.Items.Single(s => s.Id == editAvailability.SongId);
                currentSong.IsActive = editAvailability.OldValue;
                /*
                Songs.ChangeAndRefresh(currentSong.Id, s => s.IsActive, editAvailability.OldValue);
                */
                break;
            case DeleteSongCommand deleteSong:
                var oldData = deleteSong.OldData;
                _sourceSongs.Add(new SongViewModel(oldData));
                break;
            default:
                _toastManager.CreateSimpleInfoToast()
                    .WithTitle("栈中无历史操作")
                    .WithContent("没有操作可以撤回")
                    .Queue();
                break;
        }
    }

    [RelayCommand]
    private async Task CopySongAsync()
    {
        if (SelectedSong is null)
        {
            _toastManager.CreateSimpleInfoToast()
                .WithTitle("操作失败")
                .WithContent("请先选择歌曲再进行删除")
                .Queue();
            return;
        }

        var artists = string.Join('、', SelectedSong.Artists);
        var content = $"{artists} - {SelectedSong.Title}";
        var topLevel = TopLevel.GetTopLevel(
            App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null);
        if (topLevel?.Clipboard is not null)
        {
            await topLevel.Clipboard.SetTextAsync(content);
            //TODO: 提示成功
        }
        else
        {
            //TODO: Toast提醒失败
        }
    }
}