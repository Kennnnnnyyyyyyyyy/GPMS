using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Gate_Pass_management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Controllers;

[Authorize(Roles = "Admin,Reception,Security")]
public class GatePassController : Controller
{
    private readonly AppDbContext _db;
    private readonly IGatePassService _service;

    public GatePassController(AppDbContext db, IGatePassService service)
    {
        _db = db;
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var passes = await _db.GatePasses
            .OrderByDescending(g => g.Id)
            .Take(500)
            .ToListAsync();
        return View(passes);
    }

    public async Task<IActionResult> Details(int id)
    {
        var pass = await _db.GatePasses.FirstOrDefaultAsync(g => g.Id == id);
        if (pass == null) return NotFound();
        return View(pass);
    }

    public IActionResult Create(int? visitorEntryId)
    {
        ViewBag.DefaultValidFrom = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
        ViewBag.DefaultValidTo = DateTime.Now.AddHours(4).ToString("yyyy-MM-ddTHH:mm");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int? visitorEntryId, string visitorName, string mobile, DateTime? validFrom, DateTime? validTo)
    {
        if (string.IsNullOrWhiteSpace(visitorName)) ModelState.AddModelError("visitorName", "Visitor name required");
        if (string.IsNullOrWhiteSpace(mobile)) ModelState.AddModelError("mobile", "Mobile required");
        if (validFrom is null || validTo is null) ModelState.AddModelError("validFrom", "Validity window required");
        if (validFrom != null && validTo != null && validTo <= validFrom) ModelState.AddModelError("validTo", "End must be after start");
        if (!ModelState.IsValid)
        {
            ViewBag.DefaultValidFrom = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            ViewBag.DefaultValidTo = DateTime.Now.AddHours(4).ToString("yyyy-MM-ddTHH:mm");
            return View();
        }
    var pass = await _service.IssueAsync(visitorName.Trim(), mobile.Trim(), validFrom!.Value.ToUniversalTime(), validTo!.Value.ToUniversalTime(), null);
        return RedirectToAction(nameof(Details), new { id = pass.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Revoke(int id)
    {
        await _service.RevokeAsync(id);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> MarkUsed(int id)
    {
        await _service.MarkUsedAsync(id);
        return RedirectToAction(nameof(Details), new { id });
    }
}
