using System.Collections.Generic;
using System.Linq;
using GFMWakeUpHelper.Data;
using GFMWakeUpHelper.Data.Entities;

namespace GFMWakeUpHelper.App.Extensions;

public static class DbContextExtensions
{
    public static IEnumerable<IGrouping<string, Song>> GetDuplicatedSongs(this DataDbContext dbContext)
    {
        return dbContext.Songs
            .GroupBy(song => song.Title.ToLower())
            .Where(group => group.Count() > 1)
            .AsEnumerable(); // 转为 LINQ to Objects
    }
}