using Microsoft.AspNetCore.Mvc;
using NovaFit.Models;
using NovaFit.Services;

namespace NovaFit.Controllers
{
    public class AiCoachController : Controller
    {
        private readonly HuggingFaceService _aiService;

        // HuggingFaceService buraya enjekte ediliyor
        public AiCoachController(HuggingFaceService aiService)
        {
            _aiService = aiService;
        }

        // Form Sayfası
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePlan(AiRequestDto model)
        {
            // 1. Metin Tavsiyesi (Prompt aynı)
            string textPrompt = $"{model.Age} yaş, {model.Weight} kg, {model.Height} cm, {model.Gender}. " +
                                $"Hedef: {model.Goal}. Diyet ve antrenman yaz.";

            string planText = await _aiService.GetAdviceAsync(textPrompt);

            // 2. Resim İşleme
            string imageUrl = null;

            // İngilizce Prompt
            string genderEn = model.Gender == "Erkek" ? "man" : "woman";
            string goalEn = model.Goal.Contains("Kas") ? "extremely muscular bodybuilder physique" : "very fit athletic slim physique";
            string imagePrompt = $"A photo of a {genderEn} with {goalEn}, gym background, cinematic lighting, 8k";

            try
            {
                if (model.UserImage != null && model.UserImage.Length > 0)
                {
                    // A) Kullanıcı RESİM YÜKLEDİYSE -> Resimden Resim Üret (Img2Img)
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.UserImage.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();

                        // Servisteki yeni metodu çağırıyoruz
                        imageUrl = await _aiService.GenerateImageWithRefAsync(imagePrompt, imageBytes);
                    }
                }
                else
                {
                    // B) Resim Yüklemediyse -> Sıfırdan Çiz (Eski yöntem)
                    // (Eski GenerateImageAsync metodunu kullanmaya devam edebilirsin)
                    imageUrl = await _aiService.GenerateImageAsync(imagePrompt); // Serviste bu metodun durduğundan emin ol
                }
            }
            catch
            {
                planText += "<br><br>(Not: Resim servisi yoğun olduğu için oluşturulamadı.)";
            }

            var resultViewModel = new AiResponseViewModel
            {
                DietPlan = planText.Replace("\n", "<br>"),
                ImageUrl = imageUrl
            };

            return View("Result", resultViewModel);
        }
    }
}