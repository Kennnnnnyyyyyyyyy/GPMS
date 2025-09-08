using Gate_Pass_management.Data;
using Gate_Pass_management.Models;
using Gate_Pass_management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gate_Pass_management.Controllers;

[Authorize(Roles = "Admin")]
public class SmsController : Controller
{
    private readonly ISmsSender _smsSender;

    public SmsController(ISmsSender smsSender)
    {
        _smsSender = smsSender;
    }

    public async Task<IActionResult> Index()
    {
        var messages = await _smsSender.GetRecentMessagesAsync(100);
        return View(messages);
    }

    public IActionResult Send()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(string to, string message)
    {
        if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(message))
        {
            ModelState.AddModelError("", "Phone number and message are required");
            return View();
        }

        var success = await _smsSender.SendAsync(to.Trim(), message.Trim());
        
        if (success)
        {
            TempData["Success"] = "SMS sent successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            ModelState.AddModelError("", "Failed to send SMS. Check logs for details.");
            return View();
        }
    }
}
