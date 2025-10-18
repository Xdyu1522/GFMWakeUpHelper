using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GFMWakeUpHelper.Data;
using SQLitePCL;

namespace GFMWakeUpHelper.App.Commands;

public class CommandsManager
{
    private int _currentId = 1;
    private Stack<ISongEditCommand> _commands = new();

    public CommandsBuilder<T> GetCommandBuilder<T>() where T : class, ISongEditCommand, new()
    {
        return new CommandsBuilder<T>(_currentId++);
    }

    public void Register(ISongEditCommand cmd)
    {
        _commands.Push(cmd);
    }

    public async Task ExecuteAll(DataDbContext dbContext)
    {
        foreach (var i in _commands)
        {
            var result = await i.Execute(dbContext);
            if (!result)
            {
                throw new InvalidOperationException(
                    $"Command {i.CommandId} can't be executed successfully due to the non-existent song id.");
            }
        }

        Clear();
    }

    public ISongEditCommand? Undo() => _commands.Count != 0 ? _commands.Pop() : null;

    public void Clear() => _commands.Clear();
}