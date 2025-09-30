using GFMWakeUpHelper.Data.Entities;
using Microsoft.EntityFrameworkCore;

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
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}
