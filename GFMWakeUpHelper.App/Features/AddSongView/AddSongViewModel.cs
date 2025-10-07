using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GFMWakeUpHelper.App.Dialogs.AskSameSongDialog;
using GFMWakeUpHelper.Data;
using GFMWakeUpHelper.Data.Entities;
using Material.Icons;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.MessageBox;
using SukiUI.Toasts;

namespace GFMWakeUpHelper.App.Features.AddSongView;

public partial class AddSongViewModel(ISukiToastManager toastManager, ISukiDialogManager dialogManager)
    : PageBase("添加歌曲", MaterialIconKind.Abc, int.MinValue)
{
    public ObservableCollection<Song> Songs { get; } = [];

    [ObservableProperty] private string _inputSongText = string.Empty;

    private readonly DataDbContext _dbContext = new();

    partial void OnInputSongTextChanged(string value)
    {
        ParseInputText();
    }

    private void ParseInputText()
    {
        if (string.IsNullOrWhiteSpace(InputSongText))
        {
            Songs.Clear();
            return;
        }

        var lines = InputSongText.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        var parsedSongs = lines.Select((line, index) =>
        {
            var song = ParseSongLine(line);
            song.Id = index + 1; // 设置临时ID
            song.RequestedAt = DateTime.Now;
            song.IsActive = true;
            return song;
        }).Where(x => !(string.IsNullOrEmpty(x.Title) || x.Artists.Count == 0)).ToList();

        // 更新Songs集合
        Songs.Clear();
        foreach (var song in parsedSongs)
        {
            Songs.Add(song);
        }
    }

    [RelayCommand]
    private async Task Submit()
    {
        if (string.IsNullOrWhiteSpace(InputSongText))
            return;

        try
        {
            var parseResult = InputSongText.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseSongLine)
                .ToList();

            // 设置歌曲的其他属性
            var currentBatch = (_dbContext.Songs.Any() ? _dbContext.Songs.Max(s => s.Batch) : 0) + 1;
            var currentTime = DateTime.Now;
            foreach (var song in parseResult)
            {
                song.RequestedAt = currentTime;
                song.IsActive = true;
                song.Batch = currentBatch;
            }

            // 先找出没有重名的歌曲并添加
            var songsToAdd = new List<Song>();
            var songsWithSameName = new Dictionary<string, List<Song>>();

            foreach (var song in parseResult)
            {
                // 使用 EF Core 支持的方式进行不区分大小写比较
                var songTitle = song.Title.ToLower();
                var existingSong = _dbContext.Songs.FirstOrDefault(s => s.Title.ToLower() == songTitle);
                if (existingSong == null)
                {
                    // 没有重名歌曲，直接添加到待添加列表
                    songsToAdd.Add(song);
                }
                else
                {
                    // 有重名歌曲，记录下来
                    if (!songsWithSameName.ContainsKey(song.Title))
                    {
                        songsWithSameName[song.Title] = new List<Song>();
                    }

                    songsWithSameName[song.Title].Add(song);
                }
            }

            // 先添加所有没有重名的歌曲
            if (songsToAdd.Any())
            {
                _dbContext.Songs.AddRange(songsToAdd);
                await _dbContext.SaveChangesAsync();
            }

            // 处理有重名的歌曲
            foreach (var songTitle in songsWithSameName.Keys)
            {
                var pendingSongs = songsWithSameName[songTitle];
                // 从数据库中获取同名歌曲
                // 使用 EF Core 支持的方式进行不区分大小写比较
                var lowerSongTitle = songTitle.ToLower();
                var existingSongs = _dbContext.Songs.Where(s => s.Title.ToLower() == lowerSongTitle).ToList();

                // 合并两个列表显示给用户
                var allSongs = new List<Song>();
                allSongs.AddRange(existingSongs);
                allSongs.AddRange(pendingSongs);

                /*var result = await dialogManager.CreateDialog()
                    .WithTitle("合并选项")
                    .WithViewModel(dialog => new AskSameSongDialogViewModel(allSongs))
                    .OfType(NotificationType.Information)
                    /*.WithActionButton("Yes", x => { Console.WriteLine("Yes"); }, true, "Accent")
                    .WithActionButton("No", x => { Console.WriteLine("No"); }, true, "Accent")#1#
                    .TryShowAsync();*/

                var result = await ShowAskSameSongMessageBox(allSongs);
                switch (result)
                {
                    case SukiMessageBoxResult.Cancel:
                        Console.WriteLine("Canceled.");
                        await _dbContext.Songs.AddRangeAsync(pendingSongs);
                        break;
                    case SukiMessageBoxResult.No:
                        Console.WriteLine("No.");
                        await _dbContext.Songs.AddRangeAsync(pendingSongs);
                        break;
                    case SukiMessageBoxResult.Yes:
                        Console.WriteLine("Yes.");
                        break;
                }

                Console.WriteLine($"显示同名歌曲对话框: {songTitle}, 共 {allSongs.Count} 首歌曲，对话框显示结果: {result}");
            }

            // 成功提示
            InputSongText = string.Empty;
            Console.WriteLine($"成功添加 {songsToAdd.Count} 首歌曲到数据库，有 {songsWithSameName.Count} 组同名歌曲需要处理");
            toastManager.CreateSimpleInfoToast()
                .WithTitle("添加成功")
                .WithContent($"{songsToAdd.Count + songsWithSameName.Count} 首歌曲处理成功")
                .Queue();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"提交失败: {ex.Message}");
        }
    }

    private static async Task<object?> ShowAskSameSongMessageBox(IEnumerable<Song> allSongs)
    {
        var dataContext = new AskSameSongDialogViewModel(allSongs);

        var result = await SukiMessageBox.ShowDialog(new SukiMessageBoxHost
            {
                IconPreset = SukiMessageBoxIcons.Question,
                Header = "合并选项",
                Content = new AskSameSongDialog
                {
                    DataContext = dataContext
                },
                ActionButtonsPreset = SukiMessageBoxButtons.YesNoCancel
            },
            new SukiMessageBoxOptions
            {
                IsTitleBarVisible = false,
                CanResize = false
            });


        return result;
    }

    private static Song ParseSongLine(string line)
    {
        var text = line.Trim();

        // 去掉开头的序号（01.、1）、01-）
        text = Regex.Replace(text, "^\\s*\\d+[\\.\\)\\uff09-]*\\s*", "");

        // 拆分第一个连字符 → 左边 ArtistPart, 右边 Title
        var idx = text.IndexOf('-');
        string artistPart = "", titlePart = text;
        if (idx >= 0)
        {
            artistPart = text.Substring(0, idx).Trim();
            titlePart = text.Substring(idx + 1).Trim();
        }

        var song = new Song { Title = titlePart };

        // 拆分 ArtistPart（多种分隔符）
        if (!string.IsNullOrEmpty(artistPart))
        {
            var splitPattern = "\\s*(?:,|、|＆|&|/|\\band\\b|feat\\.?|ft\\.?|\\+)\\s*";
            var rawArtists = Regex.Split(artistPart, splitPattern, RegexOptions.IgnoreCase)
                .Select(a => a.Trim())
                .Where(a => !string.IsNullOrEmpty(a))
                .ToList();
            song.Artists = rawArtists;
        }

        return song;
    }
}