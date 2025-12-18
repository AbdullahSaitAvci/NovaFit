using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace NovaFit.Services
{
    public class HuggingFaceService
    {
        private readonly HttpClient _httpClient;

        // BURAYA DAHA ÖNCE ALDIĞIN "hf_..." KODUNU YAPIŞTIR (YENİSİNE GEREK YOK)
        private readonly string _apiKey = "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";

        public HuggingFaceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(3);
        }

        // 1. METİN TAVSİYESİ (Zephyr Kullanıyoruz)
        public async Task<string> GetAdviceAsync(string prompt)
        {
            // İŞTE TEK DEĞİŞEN YER BURASI:
            // Eskisi: "mistralai/Mistral-7B-Instruct-v0.2"
            // Yenisi (Zephyr):
            var modelId = "HuggingFaceH4/zephyr-7b-beta";

            var url = $"https://api-inference.huggingface.co/models/{modelId}";

            var payload = new
            {
                // Zephyr'in anladığı format biraz farklıdır, onu da ayarladım:
                inputs = $"<|system|>\nSen uzman bir spor koçusun. Türkçe cevap ver.</s>\n<|user|>\n{prompt}</s>\n<|assistant|>",
                parameters = new { max_new_tokens = 500, return_full_text = false }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode) return "Yapay zeka şu an yoğun. Lütfen tekrar deneyin.";

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseString);
            return result[0].generated_text;
        }

        // 2. RESİM METODLARI (AYNI KALIYOR)
        public async Task<string> GenerateImageWithRefAsync(string prompt, byte[] originalImageBytes)
        {
            var modelId = "runwayml/stable-diffusion-v1-5";
            var url = $"https://api-inference.huggingface.co/models/{modelId}";

            string base64Image = Convert.ToBase64String(originalImageBytes);

            var payload = new
            {
                inputs = base64Image,
                parameters = new { prompt = prompt, strength = 0.75, guidance_scale = 7.5 }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode) return null;

            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
            return $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
        }

        public async Task<string> GenerateImageAsync(string prompt)
        {
            var modelId = "runwayml/stable-diffusion-v1-5";
            var url = $"https://api-inference.huggingface.co/models/{modelId}";

            var payload = new { inputs = prompt };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode) return null;

            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
            return $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
        }
    }
}