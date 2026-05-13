using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CokDilliSozluk
{
    // API'den gelen veriyi tutacak sınıflar
    public class TranslationResponse
    {
        public ResponseData responseData { get; set; }
    }

    public class ResponseData
    {
        public string translatedText { get; set; }
    }

    class Program
    {
        // İnternet bağlantısı için tek bir HttpClient oluşturuyoruz
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("=====================================");
            Console.WriteLine("    ÇOK DİLLİ ÇEVİRİ VE SÖZLÜK     ");
            Console.WriteLine("=====================================\n");

            // Kullanıcıdan dil kodlarını alıyoruz
            Console.WriteLine("Sık kullanılan dil kodları: tr (Türkçe), en (İngilizce), de (Almanca), fr (Fransızca), es (İspanyolca)");
            Console.Write("Hangi dilden çevrilecek? (Dil kodunu girin, örn: tr): ");
            string sourceLang = Console.ReadLine()?.ToLower();

            Console.Write("Hangi dile çevrilecek? (Dil kodunu girin, örn: en): ");
            string targetLang = Console.ReadLine()?.ToLower();

            // Sonsuz bir döngü başlatıyoruz ki program hemen kapanmasın, üst üste kelime arayabilelim
            while (true)
            {
                Console.WriteLine("\n-------------------------------------");
                Console.Write($"Çevrilecek metni girin (Çıkmak için 'q' yazın): ");
                string textToTranslate = Console.ReadLine();

                // 'q' tuşuna basılırsa programdan çık
                if (textToTranslate?.ToLower() == "q")
                {
                    Console.WriteLine("Programdan çıkılıyor. Görüşmek üzere!");
                    break;
                }

                if (string.IsNullOrWhiteSpace(textToTranslate))
                {
                    Console.WriteLine("Lütfen boş bırakmayın!");
                    continue;
                }

                try
                {
                    Console.WriteLine("Çevriliyor...\n");
                    // Çeviri metodunu çağırıyoruz
                    string translatedText = await TranslateTextAsync(textToTranslate, sourceLang, targetLang);

                    Console.WriteLine($">>> Sonuç: {translatedText}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($">>> Bir hata oluştu. Lütfen internet bağlantınızı ve dil kodlarını kontrol edin.");
                    Console.WriteLine($"Hata detayı: {ex.Message}");
                }
            }
        }

        // Çeviri işlemini yapan Asenkron metod
        static async Task<string> TranslateTextAsync(string text, string fromLang, string toLang)
        {
            // URL içindeki boşluk ve özel karakterleri web formatına çeviriyoruz
            string encodedText = Uri.EscapeDataString(text);

            // MyMemory API adresini oluşturuyoruz
            string url = $"https://api.mymemory.translated.net/get?q={encodedText}&langpair={fromLang}|{toLang}";

            // API'ye gidip cevabı alıyoruz
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Gelen JSON metnini string olarak okuyoruz
            string responseBody = await response.Content.ReadAsStringAsync();

            // JSON'ı bizim C# sınıfımıza dönüştürüyoruz (Deserialize)
            var translationResult = JsonSerializer.Deserialize<TranslationResponse>(responseBody);

            return translationResult?.responseData?.translatedText ?? "Çeviri bulunamadı.";
        }
    }
}