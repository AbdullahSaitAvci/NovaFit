using Newtonsoft.Json;
using System.Text;

namespace NovaFit.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;

        // Google API Key'in (NovaFit projesi için olan ...xhD8 ile biten)
        private readonly string _apiKey = "AIzaSy.......................................";

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
        }

        public async Task<string> AnalyzeAsync(string prompt, byte[] imageBytes = null)
        {
            // 1. ÖNCELİK: Google Gemini 2.0 Flash Exp (En Kaliteli)
            var googleResult = await SendGoogleRequest(prompt);

            if (googleResult.Success)
            {
                return googleResult.Text;
            }

            // 2. YEDEK: Google hata verirse (429 Kota, 404 Bulunamadı vb.)
            // Hiç beklemeden Pollinations'a geç (OpenAI Tabanlı & Reklamı Temizlenmiş)
            var pollResult = await SendPollinationsRequest(prompt);

            return pollResult;
        }

        // --- GOOGLE GEMINI İSTEĞİ ---
        private async Task<(bool Success, string Text)> SendGoogleRequest(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent?key={_apiKey}";

            var parts = new List<object> { new { text = prompt } };
            var payload = new { contents = new[] { new { parts = parts } } };

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode) return (false, null); // Hata varsa yedek sisteme geç

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);
                string text = result.candidates[0].content.parts[0].text;

                return (true, text);
            }
            catch
            {
                return (false, null);
            }
        }

        // --- POLLINATIONS İSTEĞİ (YEDEK SİSTEM) ---
        private async Task<string> SendPollinationsRequest(string prompt)
        {
            try
            {
                // Prompt'u URL uyumlu hale getir
                // Pollinations'a "HTML Tablo yap" emrini net iletiyoruz
                string cleanPrompt = "Sen spor koçusun. Türkçe HTML Tablo formatında beslenme ve antrenman yaz. " + prompt;
                var encodedPrompt = Uri.EscapeDataString(cleanPrompt);

                var url = $"https://text.pollinations.ai/{encodedPrompt}?model=openai";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return "Bağlantı hatası oluştu.";

                string rawText = await response.Content.ReadAsStringAsync();

                // --- REKLAM TEMİZLEYİCİ (ÇÖPÇÜ) ---
                // Gelen metindeki reklamları siliyoruz
                var cleanLines = rawText.Split('\n')
                    .Where(line =>
                        !line.Contains("Pollinations") &&
                        !line.Contains("Support") &&
                        !line.Contains("**Ad**") &&
                        !line.Contains("mission") &&
                        !string.IsNullOrWhiteSpace(line)
                    );

                return string.Join("\n", cleanLines);
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        // Resim Linki (Burası zaten çalışıyor)
        public string GenerateGoalImage(string prompt)
        {
            var encodedPrompt = Uri.EscapeDataString(prompt);
            return $"https://image.pollinations.ai/prompt/{encodedPrompt}?width=1024&height=1024&nologo=true&model=flux&seed={DateTime.Now.Ticks}";
        }
    }
}