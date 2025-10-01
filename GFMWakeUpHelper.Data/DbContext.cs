using GFMWakeUpHelper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;



namespace GFMWakeUpHelper.Data;

public class DataDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Song> Songs { get; set; }
    public DbSet<PlayListWeek> PlayListWeeks { get; set; }
    public DbSet<PlayListEntry > PlayListEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(AppContext.BaseDirectory, "AppData", "WakeUpHelperData.db");
        string connStr = $"Data Source={dbPath}";
        optionsBuilder.UseSqlite(connStr);
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);

        // 给 Song.Artists 配置 JSON 序列化
        var converter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
        );

        modelBuilder.Entity<Song>()
            .Property(e => e.Artists)
            .HasConversion(converter);

        // 保留你原有的程序集配置
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}
