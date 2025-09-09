using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gate_Pass_management.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ShowRecoveryCodesModel : PageModel
    {
        public IList<string> RecoveryCodes { get; set; } = new List<string>();

        public void OnGet()
        {
            if (TempData["RecoveryCodes"] is IEnumerable<string> codes)
            {
                RecoveryCodes = new List<string>(codes);
            }
        }
    }
}
