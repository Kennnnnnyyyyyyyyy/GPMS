namespace Gate_Pass_management.Models;

public class Meeting
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? OrganizerName { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public bool IsBlocked { get; set; }

    public Room Room { get; set; } = null!;
}
