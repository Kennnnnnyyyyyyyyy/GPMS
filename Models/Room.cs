namespace Gate_Pass_management.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public string? Resources { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Location { get; set; }  // Instead of Site, use location string

    public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
    public ICollection<TimeBlock> TimeBlocks { get; set; } = new List<TimeBlock>();
}
