namespace Gate_Pass_management.Models;

public enum GatePassStatus
{
    Issued,
    Used,
    Expired,
    Revoked,
    PendingApproval,
    Active,
    Cancelled
}

public class GatePass
{
    public int Id { get; set; }
    public string PassNumber { get; set; } = null!;
    public string VisitorName { get; set; } = null!;
    public string Mobile { get; set; } = null!;
    public DateTime ValidFromUtc { get; set; }
    public DateTime ValidToUtc { get; set; }
    public GatePassStatus Status { get; set; } = GatePassStatus.Issued;
    public string? QRCodeValue { get; set; }
    // Legacy VisitorEntry relation removed
}
