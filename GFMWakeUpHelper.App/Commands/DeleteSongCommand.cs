using System.Threading.Tasks;
using GFMWakeUpHelper.Data;
using GFMWakeUpHelper.Data.Entities;

namespace GFMWakeUpHelper.App.Commands;

public class DeleteSongCommand : ISongEditCommand
{
    public int CommandId { get; set; }
    public int SongId { get; set; }
    public Song OldData { get; set; } = new();

    public DeleteSongCommand()
    {
    }

    public DeleteSongCommand(int commandId, int songId, Song oldData)
    {
        CommandId = commandId;
        SongId = songId;
        OldData = oldData;
    }

    public async Task<bool> Execute(DataDbContext dbContext)
    {
        // TODO: 判断歌曲是否存在播放信息，如果是，不允许删除
        var entity = await dbContext.Songs.FindAsync(SongId);
        if (entity == null)
            return false;
        dbContext.Songs.Remove(entity);
        return true;
    }
}