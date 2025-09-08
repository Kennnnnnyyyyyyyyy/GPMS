namespace Gate_Pass_management.Models;

public class TimeBlock
{
    public int Id { get; set; }
    public int? RoomId { get; set; }
    public string? Reason { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public Room? Room { get; set; }
}
