using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovaFit.Models;
using NovaFit.ViewModels;

namespace NovaFit.Controllers
{
    public class AccountController : Controller // Kullanıcı hesap işlemleri için controller
    {
        private readonly UserManager<AppUser> _userManager;   // Kullanıcı ekleme/silme işlemleri için
        private readonly SignInManager<AppUser> _signInManager; // Giriş/Çıkış işlemleri için

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Kullanıcıyı email ile bul
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                // Şifre kontrolü yap ve giriş yap
                // false: Oturumu kilitleme, true: Başarısız girişte kilitle (Şimdilik false)
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "E-Posta veya şifre hatalı.");
            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Yeni kullanıcı nesnesi oluştur
                var user = new AppUser
                {
                    UserName = model.Email, // Kullanıcı adı olarak E-mail kullanıyoruz
                    Email = model.Email,
                    FullName = model.FullName
                };

                // Veritabanına kaydet (Şifreyi otomatik hash'ler/şifreler)
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Başarılıysa hemen giriş yaptır
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // Hata varsa (Örn: Şifre çok kısa) listeye ekle
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // Çıkış Yap
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}