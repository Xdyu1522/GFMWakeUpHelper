using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GFMWakeUpHelper.App.Commands;
using GFMWakeUpHelper.App.Common;
using GFMWakeUpHelper.App.Converters;
using GFMWakeUpHelper.Data;
using GFMWakeUpHelper.Data.Entities;
using Material.Icons;
using Microsoft.EntityFrameworkCore;

namespace GFMWakeUpHelper.App.Features.SongManageView;

public partial class SongManageViewModel : PageBase
{
    private readonly DataDbContext _dbContext = new();
    private readonly CommandsManager _commandsManager = new();
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ObservableCollection<Song> _songs;
    [ObservableProperty] private int _recordCount;

    public SongManageViewModel() : base("歌曲条目管理", MaterialIconKind.AboutCircleOutline, -150)
    {
        ReloadData();
    }

    public void ReloadData()
    {
        if (_dbContext.Songs.Any())
        {
            var entities = _dbContext.Songs.AsNoTracking().ToList();
            Songs = new ObservableCollection<Song>(entities.Select(SongCloner.Clone));
        }
        else
        {
            Songs = [];
        }

        RecordCount = _songs.Count;
    }

    public void OnTitleChanged(int songId, string oldValue, string newValue)
    {
        var builder = _commandsManager.GetCommandBuilder<EditTitleCommand>();
        var cmd = builder.Set(s => s.SongId, songId)
            .Set(s => s.OldValue, oldValue)
            .Set(s => s.NewValue, newValue)
            .Build();
        _commandsManager.Register(cmd);
    }

    public void OnIsActiveChanged(int songId, bool oldValue, bool newValue)
    {
        var builder = _commandsManager.GetCommandBuilder<EditAvailabilityCommand>();
        var cmd = builder.Set(s => s.SongId, songId)
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


    [RelayCommand]
    private void Refresh()
    {
        ReloadData();
    }

    [RelayCommand]
    private async Task SubmitChanges()
    {
        await _commandsManager.ExecuteAll(_dbContext);
        await _dbContext.SaveChangesAsync();
    }

    [RelayCommand]
    private void CheckDuplicate()
    {
    }

    [RelayCommand]
    private void OnSearch()
    {
        // TODO: 数据库搜索逻辑
    }

    [RelayCommand]
    private void OnCheckDuplicates()
    {
        // TODO: 重复歌曲检测逻辑
    }

    [RelayCommand]
    private void OnSubmit()
    {
        // TODO: 保存修改逻辑
    }
}