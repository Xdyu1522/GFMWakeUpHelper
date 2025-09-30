using GFMWakeUpHelper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace GFMWakeUpHelper.Data.EntityConfigs;

public class PlayListEntryConfig:IEntityTypeConfiguration<PlayListEntry>
{
    public void Configure(EntityTypeBuilder<PlayListEntry> builder)
    {
        builder.ToTable("PlayListEntries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.DayOfWeek).IsRequired();
        builder.Property(x => x.Period).IsRequired();
        builder.Property(x => x.Order).IsRequired();

        builder.HasOne(e => e.Song).WithMany().HasForeignKey(e => e.SongId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.PlayListWeek).WithMany(x => x.Entries).HasForeignKey(e => e.PlayListWeekId);
    }
}