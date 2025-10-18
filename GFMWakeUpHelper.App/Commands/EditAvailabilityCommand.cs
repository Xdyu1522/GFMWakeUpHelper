using System;
using System.Threading.Tasks;
using GFMWakeUpHelper.Data;
using GFMWakeUpHelper.Data.Entities;

namespace GFMWakeUpHelper.App.Commands;

public class EditAvailabilityCommand : ISongEditCommand
{
    public int CommandId { get; set; }
    public int SongId { get; set; }
    public bool OldValue { get; set; }
    public bool NewValue { get; set; } = false;

    public EditAvailabilityCommand()
    {
        OldValue = true;
        NewValue = false;
    }

    public EditAvailabilityCommand(int commandId, int songId, bool oldValue, bool newValue)
    {
        CommandId = commandId;
        SongId = songId;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public async Task<bool> Execute(DataDbContext dbContext)
    {
        var entity = await dbContext.Songs.FindAsync(SongId);
        if (entity == null)
            return false;
        entity.IsActive = NewValue;
        return true;
    }
}