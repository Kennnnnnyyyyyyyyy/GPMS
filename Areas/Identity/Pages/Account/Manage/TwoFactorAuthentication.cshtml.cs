using Gate_Pass_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Gate_Pass_management.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class TwoFactorAuthenticationModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public TwoFactorAuthenticationModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public bool Is2faEnabled { get; set; }
        public bool HasAuthenticator { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Is2faEnabled = false;
                HasAuthenticator = false;
                return;
            }
            Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            HasAuthenticator = (await _userManager.GetAuthenticatorKeyAsync(user)) != null;
        }
    }
}
