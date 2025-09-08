using Gate_Pass_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gate_Pass_management.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly UrlEncoder _urlEncoder;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, UrlEncoder urlEncoder)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Console.WriteLine($"Login attempt for user: {model.UserName}");
            
            if (!ModelState.IsValid) 
            {
                Console.WriteLine("ModelState is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Model error: {error.ErrorMessage}");
                }
                return View(model);
            }
            
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                Console.WriteLine($"User {model.UserName} not found");
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View(model);
            }
            
            Console.WriteLine($"User {model.UserName} found, attempting sign in");
            
            // Sign out any existing session first
            await _signInManager.SignOutAsync();
            
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: false);
            
            Console.WriteLine($"Sign in result: Succeeded={result.Succeeded}, RequiresTwoFactor={result.RequiresTwoFactor}, IsLockedOut={result.IsLockedOut}");
            
            if (result.Succeeded) 
            {
                Console.WriteLine("Login successful, redirecting to admin");
                // Double-check the user is signed in
                if (_signInManager.IsSignedIn(User))
                {
                    return LocalRedirect(model.ReturnUrl ?? "/Admin/Dashboard");
                }
                else
                {
                    Console.WriteLine("User not signed in after PasswordSignInAsync, forcing sign in");
                    // Force sign in if somehow it didn't work
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return LocalRedirect(model.ReturnUrl ?? "/Admin/Dashboard");
                }
            }
            
            if (result.RequiresTwoFactor) 
            {
                Console.WriteLine("2FA required");
                return RedirectToAction("LoginWith2fa", new { ReturnUrl = model.ReturnUrl });
            }
            
            Console.WriteLine("Login failed");
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult LoginWith2fa(string? returnUrl = null) => View(new LoginWith2faViewModel { ReturnUrl = returnUrl });

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) return RedirectToAction("Login");
            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, false, false);
            if (result.Succeeded) return LocalRedirect(model.ReturnUrl ?? Url.Action("Index", "Home")!);
            ModelState.AddModelError(string.Empty, "Invalid authenticator code");
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var model = new ManageViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var model = new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            return View(model);
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Verification code is invalid.");
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            TempData["StatusMessage"] = "Your authenticator app has been verified.";
            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                TempData["RecoveryCodes"] = recoveryCodes;
                return RedirectToAction("ShowRecoveryCodes");
            }
            return RedirectToAction("Manage");
        }

        [Authorize]
        public IActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (IEnumerable<string>?)TempData["RecoveryCodes"];
            if (recoveryCodes == null) return RedirectToAction("Manage");
            return View(new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes });
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded) throw new InvalidOperationException("Unexpected error occurred disabling 2FA");
            TempData["StatusMessage"] = "2fa has been disabled";
            return RedirectToAction("Manage");
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(AppUser user, EnableAuthenticatorViewModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            model.SharedKey = FormatKey(unformattedKey!);
            var email = await _userManager.GetEmailAsync(user);
            model.AuthenticatorUri = GenerateQrCodeUri(email ?? user.UserName!, unformattedKey!);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.AsSpan(currentPosition));
            }
            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
            return string.Format(AuthenticatorUriFormat, _urlEncoder.Encode("GPMS"), _urlEncoder.Encode(email), unformattedKey);
        }
    }

    public class LoginViewModel
    {
        public string? ReturnUrl { get; set; }
        [Required] public string UserName { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }

    public class LoginWith2faViewModel
    {
        public string? ReturnUrl { get; set; }
        [Required, Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; } = string.Empty;
    }

    public class ManageViewModel
    {
        public bool HasAuthenticator { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int RecoveryCodesLeft { get; set; }
    }

    public class EnableAuthenticatorViewModel
    {
        [Required, Display(Name = "Verification Code")]
        public string Code { get; set; } = string.Empty;
        public string SharedKey { get; set; } = string.Empty;
        public string AuthenticatorUri { get; set; } = string.Empty;
    }

    public class ShowRecoveryCodesViewModel
    {
        public IEnumerable<string> RecoveryCodes { get; set; } = new List<string>();
    }
}
