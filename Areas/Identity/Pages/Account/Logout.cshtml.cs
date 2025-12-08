using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFit.Models;

namespace NovaFit.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<AppUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        // Eðer yanlýþlýkla linke týklayarak (GET) gelirlerse de çýkýþ yapsýn (Ýsteðe baðlý, garanti olsun diye)
        public async Task<IActionResult> OnGet()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Kullanýcý çýkýþ yaptý.");
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // Asýl çalýþan kýsým burasý (Butona basýnca)
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Kullanýcý çýkýþ yaptý.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }
    }
}