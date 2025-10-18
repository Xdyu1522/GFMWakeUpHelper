using System.Threading.Tasks;
using GFMWakeUpHelper.Data;

namespace GFMWakeUpHelper.App.Commands;

public class EditTitleCommand : ISongEditCommand
{
    public int CommandId { get; set; }
    public int SongId { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }

    public EditTitleCommand()
    {
        OldValue = string.Empty;
        NewValue = string.Empty;
    }

    public EditTitleCommand(int commandId, int songId, string oldValue, string newValue)
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
        entity.Title = NewValue;
        return true;
    }
}