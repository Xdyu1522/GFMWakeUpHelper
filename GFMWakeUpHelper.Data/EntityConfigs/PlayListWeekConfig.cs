using GFMWakeUpHelper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFMWakeUpHelper.Data.EntityConfigs;

public class PlayListWeekConfig: IEntityTypeConfiguration<PlayListWeek>
{
    public void Configure(EntityTypeBuilder<PlayListWeek> builder)
    {
        builder.ToTable("PlayListWeeks");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.WeekStart).IsRequired();
        builder.Property(x => x.WeekEnd).IsRequired();
        
        builder.HasMany(x => x.Entries).WithOne(x => x.PlayListWeek).HasForeignKey(x => x.PlayListWeekId).OnDelete(DeleteBehavior.Cascade);
    }
}