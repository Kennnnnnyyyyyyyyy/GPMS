namespace Gate_Pass_management.Services;

public interface ISmsSender
{
    Task<bool> SendAsync(string to, string message, CancellationToken ct = default);
    Task<List<Gate_Pass_management.Models.SmsMessage>> GetRecentMessagesAsync(int limit = 50, CancellationToken ct = default);
}
