using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Services;

public class GatePassService : IGatePassService
{
    private readonly AppDbContext _db;
    private readonly Random _rng = new();

    public GatePassService(AppDbContext db)
    {
        _db = db;
    }

    public string GeneratePassNumber()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // exclude easily confused
        Span<char> buffer = stackalloc char[8];
        for (int i = 0; i < buffer.Length; i++) buffer[i] = chars[_rng.Next(chars.Length)];
        return new string(buffer);
    }

    public async Task<GatePass> IssueAsync(string visitorName, string mobile, DateTime validFromUtc, DateTime validToUtc, int? visitorEntryId = null, CancellationToken ct = default)
    {
        if (validToUtc <= validFromUtc) throw new ArgumentException("validToUtc must be after validFromUtc");
        // ensure unique pass number (retry few times)
        string passNumber;
        int attempts = 0;
        do
        {
            passNumber = GeneratePassNumber();
            attempts++;
        } while (attempts < 5 && await _db.GatePasses.AnyAsync(g => g.PassNumber == passNumber, ct));
        if (await _db.GatePasses.AnyAsync(g => g.PassNumber == passNumber, ct))
            throw new InvalidOperationException("Failed to generate unique pass number after retries");

    var gp = new GatePass
        {
            PassNumber = passNumber,
            VisitorName = visitorName,
            Mobile = mobile,
            ValidFromUtc = validFromUtc,
            ValidToUtc = validToUtc,
            Status = GatePassStatus.Issued,
            QRCodeValue = passNumber // placeholder; later replace with actual QR payload
        };
        _db.GatePasses.Add(gp);
        await _db.SaveChangesAsync(ct);
        return gp;
    }

    public async Task<bool> RevokeAsync(int id, CancellationToken ct = default)
    {
        var gp = await _db.GatePasses.FindAsync(new object?[] { id }, ct);
        if (gp == null) return false;
        if (gp.Status is GatePassStatus.Used or GatePassStatus.Revoked) return false;
        gp.Status = GatePassStatus.Revoked;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> MarkUsedAsync(int id, CancellationToken ct = default)
    {
        var gp = await _db.GatePasses.FindAsync(new object?[] { id }, ct);
        if (gp == null) return false;
        if (gp.Status != GatePassStatus.Issued) return false;
        gp.Status = GatePassStatus.Used;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
