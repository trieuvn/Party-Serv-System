using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace eParty.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly string GEMINI_API_KEY = "AIzaSyCcH9_hZxXeuVfhLfQohE7mPnruznSJZ4E";
        private readonly string GEMINI_API_URL = "https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent";

        [HttpPost]
        public async Task<JsonResult> Ask(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Json(new { reply = "⚠️ Bạn chưa nhập tin nhắn." });

            try
            {
                using (var client = new HttpClient())
                {
                    // ✅ Model mới: gemini-2.5-flash (stable, nhanh, hỗ trợ tốt)
                    var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-2.5-flash:generateContent?key={GEMINI_API_KEY}";

                    // ✅ Payload chuẩn (không thay đổi)
                    var json = new JObject
                    {
                        ["contents"] = new JArray(
                            new JObject
                            {
                                ["role"] = "user",
                                ["parts"] = new JArray(
                                    new JObject { ["text"] = message }
                                )
                            }
                        )
                    };

                    var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        return Json(new { reply = $"❌ Lỗi API: {response.StatusCode} - {error}" });
                    }

                    var responseText = await response.Content.ReadAsStringAsync();
                    var obj = JObject.Parse(responseText);

                    // ✅ Đọc kết quả chuẩn (không thay đổi)
                    var reply = obj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    if (string.IsNullOrEmpty(reply))
                        reply = "❌ Không nhận được phản hồi từ Gemini.";

                    return Json(new { reply });
                }
            }
            catch (Exception ex)
            {
                return Json(new { reply = $"⚠️ Lỗi: {ex.Message}" });
            }
        }
    }
}
