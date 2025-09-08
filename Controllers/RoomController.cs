using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Controllers;

[Authorize(Roles = "Admin")]
public class RoomController : Controller
{
    private readonly AppDbContext _context;
    public RoomController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var rooms = await _context.Rooms.OrderBy(r => r.Name).ToListAsync();
        return View(rooms);
    }

    public IActionResult Create()
    {
        return View(new Room());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Room room)
    {
        if (!ModelState.IsValid)
        {
            return View(room);
        }
        _context.Add(room);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound();
        return View(room);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Room room)
    {
        if (id != room.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            return View(room);
        }
        _context.Update(room);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (room == null) return NotFound();
        return View(room);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (room == null) return NotFound();
        return View(room);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room != null)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
