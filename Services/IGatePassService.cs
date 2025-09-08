using Gate_Pass_management.Models;

namespace Gate_Pass_management.Services;

public interface IGatePassService
{
    Task<GatePass> IssueAsync(string visitorName, string mobile, DateTime validFromUtc, DateTime validToUtc, int? visitorEntryId = null, CancellationToken ct = default);
    Task<bool> RevokeAsync(int id, CancellationToken ct = default);
    Task<bool> MarkUsedAsync(int id, CancellationToken ct = default);
    string GeneratePassNumber();
}
