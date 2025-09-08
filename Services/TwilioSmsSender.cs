using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Microsoft.EntityFrameworkCore;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Gate_Pass_management.Services;

public class TwilioSmsSender : ISmsSender
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<TwilioSmsSender> _logger;

    public TwilioSmsSender(AppDbContext db, IConfiguration config, ILogger<TwilioSmsSender> logger)
    {
        _db = db;
        _config = config;
        _logger = logger;
        
        var accountSid = _config["Twilio:AccountSid"];
        var authToken = _config["Twilio:AuthToken"];
        if (!string.IsNullOrEmpty(accountSid) && !string.IsNullOrEmpty(authToken))
        {
            TwilioClient.Init(accountSid, authToken);
        }
    }

    public async Task<bool> SendAsync(string to, string message, CancellationToken ct = default)
    {
        var smsRecord = new SmsMessage
        {
            To = to,
            Body = message,
            Status = "Pending",
            CreatedUtc = DateTime.UtcNow
        };

        try
        {
            var fromNumber = _config["Twilio:FromNumber"];
            if (string.IsNullOrEmpty(fromNumber))
            {
                _logger.LogError("Twilio FromNumber not configured");
                smsRecord.Status = "Failed";
                smsRecord.Error = "FromNumber not configured";
                _db.SmsMessages.Add(smsRecord);
                await _db.SaveChangesAsync(ct);
                return false;
            }

            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(fromNumber),
                to: new PhoneNumber(to)
            );

            smsRecord.ProviderMessageId = messageResource.Sid;
            smsRecord.Status = messageResource.Status.ToString();
            smsRecord.SentUtc = DateTime.UtcNow;

            _logger.LogInformation("SMS sent successfully to {To}, SID: {Sid}", to, messageResource.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {To}", to);
            smsRecord.Status = "Failed";
            smsRecord.Error = ex.Message;
        }

        _db.SmsMessages.Add(smsRecord);
        await _db.SaveChangesAsync(ct);
        return smsRecord.Status != "Failed";
    }

    public Task<List<SmsMessage>> GetRecentMessagesAsync(int limit = 50, CancellationToken ct = default)
    {
        return _db.SmsMessages
            .OrderByDescending(s => s.CreatedUtc)
            .Take(limit)
            .ToListAsync(ct);
    }
}
