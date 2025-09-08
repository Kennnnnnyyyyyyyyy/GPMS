using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Gate_Pass_management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Controllers;

[Authorize(Roles = "Admin,Reception")] 
public class MeetingController : Controller
{
    private readonly AppDbContext _db;
    private readonly ISchedulingService _sched;
    public MeetingController(AppDbContext db, ISchedulingService sched)
    {
        _db = db; _sched = sched;
    }

    public async Task<IActionResult> Index(int? roomId)
    {
        ViewBag.Rooms = await _db.Rooms.OrderBy(r=>r.Name).ToListAsync();
        var q = _db.Meetings.Include(m=>m.Room).AsQueryable();
        if (roomId.HasValue) q = q.Where(m => m.RoomId == roomId.Value);
        var list = await q.OrderByDescending(m=>m.StartUtc).Take(200).ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Rooms = await _db.Rooms.OrderBy(r=>r.Name).ToListAsync();
        return View(new Meeting { StartUtc = DateTime.UtcNow.AddMinutes(5), EndUtc = DateTime.UtcNow.AddMinutes(65) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Meeting meeting)
    {
        if (meeting.EndUtc <= meeting.StartUtc)
            ModelState.AddModelError("EndUtc", "End must be after Start");
        if (!await _db.Rooms.AnyAsync(r=>r.Id==meeting.RoomId))
            ModelState.AddModelError("RoomId", "Room not found");
        if (ModelState.IsValid)
        {
            if (await _sched.HasConflictAsync(meeting.RoomId, meeting.StartUtc, meeting.EndUtc))
                ModelState.AddModelError("StartUtc", "Time conflict with existing meeting");
            else
            {
                _db.Meetings.Add(meeting);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        ViewBag.Rooms = await _db.Rooms.OrderBy(r=>r.Name).ToListAsync();
        return View(meeting);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var meeting = await _db.Meetings.FindAsync(id);
        if (meeting == null) return NotFound();
        ViewBag.Rooms = await _db.Rooms.OrderBy(r=>r.Name).ToListAsync();
        return View(meeting);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Meeting meeting)
    {
        if (id != meeting.Id) return NotFound();
        if (meeting.EndUtc <= meeting.StartUtc)
            ModelState.AddModelError("EndUtc", "End must be after Start");
        if (ModelState.IsValid)
        {
            if (await _sched.HasConflictAsync(meeting.RoomId, meeting.StartUtc, meeting.EndUtc, meeting.Id))
                ModelState.AddModelError("StartUtc", "Time conflict with existing meeting");
            else
            {
                _db.Update(meeting);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        ViewBag.Rooms = await _db.Rooms.OrderBy(r=>r.Name).ToListAsync();
        return View(meeting);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var meeting = await _db.Meetings.Include(m=>m.Room).FirstOrDefaultAsync(m=>m.Id==id);
        if (meeting == null) return NotFound();
        return View(meeting);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var meeting = await _db.Meetings.FindAsync(id);
        if (meeting != null)
        {
            _db.Meetings.Remove(meeting);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
