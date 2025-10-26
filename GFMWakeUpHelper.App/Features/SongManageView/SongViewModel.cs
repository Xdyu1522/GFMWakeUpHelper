using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using GFMWakeUpHelper.Data.Entities;

namespace GFMWakeUpHelper.App.Features.SongManageView;

public class SongViewModel : ObservableObject
{
    private readonly Song _inner;

    public SongViewModel(Song song)
    {
        _inner = song;
    }

    public int Id => _inner.Id;
    public int Batch => _inner.Batch;
    public DateTime RequestedAt => _inner.RequestedAt;

    public Song ToSong() => _inner;

    public string Title
    {
        get => _inner.Title;
        set
        {
            if (_inner.Title != value)
            {
                _inner.Title = value;
                OnPropertyChanged();
            }
        }
    }

    public List<string> Artists
    {
        get => _inner.Artists;
        set
        {
            if (_inner.Artists != value)
            {
                _inner.Artists = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsActive
    {
        get => _inner.IsActive;
        set
        {
            if (_inner.IsActive != value)
            {
                _inner.IsActive = value;
                OnPropertyChanged();
            }
        }
    }
}