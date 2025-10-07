using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GFMWakeUpHelper.App.Commands;
using GFMWakeUpHelper.Data;
using GFMWakeUpHelper.Data.Entities;
using Material.Icons;

namespace GFMWakeUpHelper.App.Features.SongManageView;

public partial class SongManageViewModel : PageBase
{
    private readonly DataDbContext _dbContext = new();
    [ObservableProperty] private ObservableCollection<IChangeSongSetCommand> _commands = [];
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ObservableCollection<Song> _songs;
    [ObservableProperty] private int _recordCount;

    public SongManageViewModel() : base("歌曲条目管理", MaterialIconKind.AboutCircleOutline, -150)

    {
        Songs = new(_dbContext.Songs.Any() ? _dbContext.Songs.ToList() : []);
        RecordCount = _songs.Count;
    }

    public void ReloadData()
    {
        Songs = new(_dbContext.Songs.Any() ? _dbContext.Songs.ToList() : []);
        RecordCount = Songs.Count;
    }

    [RelayCommand]
    private async Task OnIsActiveChanged(Tuple<Song, bool>? parameters)
    {
        if (parameters == null)
            return;
        var currentCommand = await new SetAvailabilityCommandBuilder(_dbContext)
            .WithSongId(parameters.Item1.Id)
            .SetTo(parameters.Item2)
            .Build();
        Commands.Add(currentCommand);
    }


    [RelayCommand]
    private void Refresh()
    {
        ReloadData();
    }

    [RelayCommand]
    private async Task SubmitChanges()
    {
        foreach (var cmd in Commands)
        {
            await cmd.ExecuteAsync();
        }

        Commands.Clear();
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