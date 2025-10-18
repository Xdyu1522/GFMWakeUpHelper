using GFMWakeUpHelper.Data.Entities;

namespace GFMWakeUpHelper.App.Common;

public static class SongCloner
{
    public static Song Clone(Song src) => new()
    {
        Id = src.Id,
        Title = src.Title,
        Artists = src.Artists,
        IsActive = src.IsActive,
        RequestedAt = src.RequestedAt,
        Batch = src.Batch
    };
}