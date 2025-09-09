using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Gate_Pass_management.Models;

namespace Gate_Pass_management.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class GenerateRecoveryCodesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public GenerateRecoveryCodesModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IList<string> RecoveryCodes { get; set; } = new List<string>();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }

            var codes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            RecoveryCodes = new List<string>(codes);
            TempData["RecoveryCodes"] = RecoveryCodes.ToArray();
            return RedirectToPage("./ShowRecoveryCodes");
        }
    }
}
