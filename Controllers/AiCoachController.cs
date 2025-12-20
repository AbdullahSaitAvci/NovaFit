using Microsoft.AspNetCore.Mvc;
using NovaFit.Models;
using NovaFit.Services;

namespace NovaFit.Controllers
{
    public class AiCoachController : Controller
    {
        private readonly GeminiService _geminiService;

        public AiCoachController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePlan(AiRequestDto model)
        {
            // --- GEMINI 2.0 İÇİN DETAYLI PROMPT ---
            string textPrompt = $@"
                Sen dünya çapında uzman bir spor koçusun.
                Danışan Bilgileri: {model.Age} yaş, {model.Weight}kg, {model.Height}cm, {model.Gender}.
                Hedef: {model.Goal}. Seviye: {model.FitnessLevel}.

                Görevin: Bu kişi için 1 GÜNLÜK ÖRNEK BESLENME ve ANTRENMAN planı hazırla.
                
                KURALLAR:
                1. Cevabı SADECE HTML kodu olarak ver. (Markdown, ```html veya ``` İŞARETLERİNİ KULLANMA).
                2. Beslenme planını bir HTML TABLOSU (table class='table table-bordered table-sm') olarak hazırla.
                3. Antrenman planını maddeler halinde (ul/li) hazırla.
                4. Motive edici, emojili ve profesyonel bir dil kullan.

                Aşağıdaki HTML Şablonunu doldurarak cevap ver:
                
                <div class='ai-result'>
                   <h4 class='text-primary'><i class='bi bi-activity'></i> Vücut Analizi</h4>
                   <p>...Kişiye özel kısa analiz ve BMI yorumu...</p>
                   
                   <h4 class='text-success mt-4'><i class='bi bi-egg-fried'></i> Örnek Beslenme Planı</h4>
                   <table class='table table-bordered table-sm'>
                       <thead class='table-light'><tr><th>Öğün</th><th>Öneri</th><th>Tahmini Kalori</th></tr></thead>
                       <tbody>
                           <tr><td><b>Kahvaltı</b></td><td>...detay...</td><td>...kcal...</td></tr>
                           <tr><td><b>Öğle</b></td><td>...detay...</td><td>...kcal...</td></tr>
                           <tr><td><b>Ara Öğün</b></td><td>...detay...</td><td>...kcal...</td></tr>
                           <tr><td><b>Akşam</b></td><td>...detay...</td><td>...kcal...</td></tr>
                       </tbody>
                   </table>

                   <h4 class='text-danger mt-4'><i class='bi bi-lightning-charge'></i> Antrenman Programı</h4>
                   <ul class='list-group list-group-flush'>
                       <li class='list-group-item'><b>Isınma:</b> ...</li>
                       <li class='list-group-item'><b>Ana Set:</b> ...</li>
                       <li class='list-group-item'><b>Soğuma:</b> ...</li>
                   </ul>
                   
                   <div class='alert alert-info mt-3'>
                        <i class='bi bi-info-circle'></i> <b>Koçun Notu:</b> ...Motive edici kapanış cümlesi...
                   </div>
                </div>
            ";

            string planText = "";
            string imageUrl = "";

            try
            {
                // Görsel Prompt (Rastgelelik içerir)
                string[] actions = new[] { "drinking water", "resting on bench", "running on treadmill", "lifting dumbbells", "stretching" };
                var rand = new Random();
                string randomAction = actions[rand.Next(actions.Length)];

                string genderEn = model.Gender == "Erkek" ? "man" : "woman";
                // İleri seviye seçerse kaslı, başlangıç seçerse fit olsun
                string bodyType = model.FitnessLevel.Contains("İleri") ? "muscular bodybuilder" : "fit athletic";

                string imagePrompt = $"A realistic photo of a {model.Age} year old {genderEn}, {bodyType}, wearing sportswear, {randomAction} in gym, cinematic lighting, 8k detailed";

                // --- SERVİSLERİ ÇAĞIR ---

                // 1. Gemini 2.0 (Metin - Tablolu)
                planText = await _geminiService.AnalyzeAsync(textPrompt, null);

                // 2. Pollinations (Resim)
                imageUrl = _geminiService.GenerateGoalImage(imagePrompt);
            }
            catch (Exception ex)
            {
                planText = "<div class='alert alert-danger'>Hata: " + ex.Message + "</div>";
            }

            var resultViewModel = new AiResponseViewModel
            {
                DietPlan = planText,
                ImageUrl = imageUrl
            };

            return View("Result", resultViewModel);
        }
    }
}