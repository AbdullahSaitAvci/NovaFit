using System.ComponentModel.DataAnnotations;

namespace NovaFit.ViewModels
{
    public class LoginViewModel // Kullanıcı giriş işlemi için gerekli verileri tutan ViewModel
    {
        [Required(ErrorMessage = "E-Posta zorunludur.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}