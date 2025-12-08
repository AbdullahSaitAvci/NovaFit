using Microsoft.AspNetCore.Identity.UI.Services;

namespace NovaFit.Services
{
    // Bu sınıf, mail atmak yerine linki OUTPUT penceresine yazar.
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Mail içeriğini ve linki Visual Studio'nun "Output" ekranına basar
            System.Diagnostics.Debug.WriteLine($"\n================ EMAIL SİMÜLASYONU ================");
            System.Diagnostics.Debug.WriteLine($"KİME: {email}");
            System.Diagnostics.Debug.WriteLine($"KONU: {subject}");
            System.Diagnostics.Debug.WriteLine($"İÇERİK: {htmlMessage}");
            System.Diagnostics.Debug.WriteLine($"===================================================\n");

            return Task.CompletedTask;
        }
    }
}