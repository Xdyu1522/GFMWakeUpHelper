using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GFMWakeUpHelper.Data.Entities;

public class Song
{
    public int Id { get; set; } // Primary Key
    public string Title { get; set; } = string.Empty;
    public List<string> Artists { get; set; } = [];
    public int Batch { get; set; } // 点歌批次
    public DateTime RequestedAt { get; set; } // 点歌时间
    public bool IsActive { get; set; } = true; // 是否在当前候选队列
    
}