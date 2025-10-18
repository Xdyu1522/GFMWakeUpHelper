using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GFMWakeUpHelper.App.Commands;

public class CommandsBuilder<T>(int id) where T : class, ISongEditCommand, new()
{
    private readonly T _command = new T { CommandId = id };

    public CommandsBuilder<T> ForSong(int songId)
    {
        _command.SongId = songId;
        return this;
    }

    public CommandsBuilder<T> Set<TValue>(Expression<Func<T, TValue>> property, TValue value)
    {
        if (property.Body is MemberExpression member && member.Member is PropertyInfo propInfo)
        {
            if (!propInfo.CanWrite)
                throw new InvalidOperationException("Property '{propInfo.Name}' does not have a public setter");
            propInfo.SetValue(_command, value);
        }
        else
        {
            throw new InvalidOperationException("Expression must be a property access");
        }

        return this;
    }

    public T Build() => _command;
}