namespace Gate_Pass_management.Models;

public class SmsMessage
{
    public int Id { get; set; }
    public string To { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string? ProviderMessageId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Sent, Failed
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? SentUtc { get; set; }
    public string? Error { get; set; }
}
