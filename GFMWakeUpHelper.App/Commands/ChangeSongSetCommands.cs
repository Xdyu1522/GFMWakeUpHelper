using System;
using System.Threading.Tasks;
using GFMWakeUpHelper.Data;
using GFMWakeUpHelper.Data.Entities;

namespace GFMWakeUpHelper.App.Commands;

public class SetAvailabilityCommand : IChangeSongSetCommand
{
    private readonly int _songID;
    private readonly bool _newValue;
    private bool _oldValue;
    private Song? _currentSong;
    private readonly DataDbContext _dbContext;

    internal SetAvailabilityCommand(int songID, bool newValue, DataDbContext dbContext)
    {
        _songID = songID;
        _newValue = newValue;
        _dbContext = dbContext;
    }

    protected internal async Task<bool> Build()
    {
        var song = await _dbContext.Songs.FindAsync(_songID);
        if (song == null)
            return false;
        _oldValue = song.IsActive;
        _currentSong = song;
        return true;
    }

    public async Task<bool> ExecuteAsync()
    {
        if (_currentSong != null)
        {
            _currentSong.IsActive = _newValue;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public Task UndoAsync()
    {
        throw new System.NotImplementedException();
    }
}

public class SetAvailabilityCommandBuilder(DataDbContext dbContext)
{
    private int? _songId;
    private bool? _newValue;

    public SetAvailabilityCommandBuilder WithSongId(int id)
    {
        _songId = id;
        return this;
    }

    public SetAvailabilityCommandBuilder SetTo(bool newValue)
    {
        _newValue = newValue;
        return this;
    }

    public async Task<SetAvailabilityCommand> Build()
    {
        if (_songId == null)
            throw new InvalidOperationException("Song ID must be specified.");
        if (_newValue == null)
            throw new InvalidOperationException("New value must be specified.");

        var command = new SetAvailabilityCommand(_songId.Value, _newValue.Value, dbContext);
        var result = await command.Build();
        if (result)
        {
            return command;
        }

        throw new InvalidOperationException("SongID does not exist.");
    }
}