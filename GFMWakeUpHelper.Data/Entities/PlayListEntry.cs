namespace GFMWakeUpHelper.Data.Entities;

public class PlayListEntry
{ 
        public int Id { get; set; }
        
        public int SongId { get; set; }       // 外键字段
        public Song? Song { get; set; }       // 导航属性

        public int PlayListWeekId { get; set; }
        public PlayListWeek? PlayListWeek { get; set; }

        public DayOfWeekEnum DayOfWeek { get; set; }
        public Period Period { get; set; }
        public int Order { get; set; }
}