using GFMWakeUpHelper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFMWakeUpHelper.Data.EntityConfigs;

public class SongEntityConfig: IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.ToTable("Songs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.Property(s => s.Title).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Artists).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Batch).IsRequired();
        builder.Property(s => s.RequestedAt).IsRequired();
        builder.Property(s => s.IsActive).IsRequired();
    }
}