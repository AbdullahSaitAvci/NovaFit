namespace NovaFit.Models
{
    // Kullanıcıdan form aracılığıyla alacağımız veriler
    public class AiRequestDto
    {
        public int Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public string Gender { get; set; }       // Erkek / Kadın
        public string Goal { get; set; }         // Kilo ver / Kas yap
        public string FitnessLevel { get; set; } // Başlangıç / İleri               
        public IFormFile? UserImage { get; set; } // Kullanıcının yükleyeceği fotoğraf

    }

    // Sonuç sayfasında ekrana basacağımız veriler
    public class AiResponseViewModel
    {
        public string DietPlan { get; set; }    // Yapay zekanın yazdığı metin
        public string ImageUrl { get; set; }    // Yapay zekanın çizdiği resim (Base64)
    }
}