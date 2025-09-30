namespace GFMWakeUpHelper.Data.Entities;

public class PlayListWeek
{
    public int Id {get; set;}
    public DateTime WeekStart {get; set;}
    public DateTime WeekEnd {get; set;}
    public ICollection<PlayListEntry> Entries { get; set; } = new List<PlayListEntry>();
}