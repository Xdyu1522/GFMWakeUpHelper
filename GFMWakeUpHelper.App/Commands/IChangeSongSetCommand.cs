using System.Threading.Tasks;
using GFMWakeUpHelper.Data;

namespace GFMWakeUpHelper.App.Commands;

public interface IChangeSongSetCommand
{
    public Task<bool> ExecuteAsync();
    public Task UndoAsync();
}