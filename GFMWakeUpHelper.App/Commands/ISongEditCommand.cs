using System.Threading.Tasks;
using GFMWakeUpHelper.Data;

namespace GFMWakeUpHelper.App.Commands;

public interface ISongEditCommand
{
    public int CommandId { get; set; }
    public int SongId { get; set; }
    public Task<bool> Execute(DataDbContext dbContext);
}