using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using eParty.Models;
using System.Linq;
using System.Collections.Generic;

namespace eParty.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly string GEMINI_API_KEY = "AIzaSyAPgYAu0A0DN4PmNz3-p3pi3X20pJeZx6A";

        // Giữ nguyên model v1/gemini-2.0-flash của bạn
        private readonly string GEMINI_API_URL = "https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent";

        private AppDbContext db = new AppDbContext();

        [HttpPost]
        public async Task<JsonResult> Ask(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Json(new { reply = "⚠️ Bạn chưa nhập tin nhắn." });

            try
            {
                // === BƯỚC 1: ĐỊNH NGHĨA SYSTEM PROMPT ===
                string systemPrompt = @"Bạn là ""eParty Assistant"", trợ lý AI thân thiện và chuyên nghiệp của website đặt tiệc eParty.

Nhiệm vụ của bạn là hỗ trợ người dùng tìm hiểu về dịch vụ và hướng dẫn họ cách đặt tiệc.
Luôn tuân thủ các quy tắc sau:

1.  **Về eParty:** Chúng ta là một nền tảng cung cấp dịch vụ đặt tiệc trọn gói, bao gồm các menu (gói tiệc) và các món ăn (food) đa dạng.
2.  **Cách Đăng nhập/Đặt tiệc:** Để ""Đăng nhập"" hoặc ""Đặt tiệc"", người dùng cần nhấn vào các nút ở **góc trên bên phải** của trang web.
3.  **Quy tắc bắt buộc:** Người dùng **BẮT BUỘC phải đăng nhập** bằng tài khoản trước khi có thể tiến hành đặt tiệc.
4.  **Phạm vi:** Chỉ trả lời các câu hỏi liên quan đến eParty, dịch vụ, menu, món ăn, và cách thức đặt tiệc. Lịch sự từ chối các câu hỏi không liên quan (ví dụ: hỏi về chính trị, thời tiết, v.v.).

Trụ sở của eParty đặt tại 141 UEF, Điện Biên Phủ, Phường 15, Quận Bình Thạnh, TP.HCM.
Thời gian hiện tại là " + DateTime.Now.ToString("HH:mm dd/MM/yyyy") + @".
Hãy luôn giữ giọng điệu vui vẻ và nhiệt tình!";

                // === BƯỚC 2: TRUY VẤN DỮ LIỆU NẾU CẦN (RAG) ===
                string contextData = "";
                if (message.ToLower().Contains("menu") || message.ToLower().Contains("thực đơn"))
                {
                    var menus = db.Menus.Where(m => m.Status == "Active").ToList();
                    contextData = "\n\nThông tin thêm: Dưới đây là các menu (gói tiệc) có sẵn:\n";
                    foreach (var menu in menus)
                    {
                        contextData += $"- {menu.Name}: {menu.Cost.ToString("N0")} VNĐ. Mô tả: {menu.Description}\n";
                    }
                }
                else if (message.ToLower().Contains("món ăn") || message.ToLower().Contains("food"))
                {
                    var foods = db.Foods.Take(20).ToList(); // Lấy 20 món ăn
                    contextData = "\n\nThông tin thêm: Dưới đây là một số món ăn có sẵn:\n";
                    foreach (var food in foods)
                    {
                        contextData += $"- {food.Name}: {food.Cost.ToString("N0")} VNĐ\n";
                    }
                }

                using (var client = new HttpClient())
                {
                    var url = $"{GEMINI_API_URL}?key={GEMINI_API_KEY}";

                    // === BƯỚC 3: GẮN PROMPT VÀO USER TEXT (CHO API v1) ===
                    // Gộp chung prompt hệ thống, dữ liệu CSDL và câu hỏi của người dùng
                    string fullUserMessage = $"{systemPrompt}\n\n---DỮ LIỆU BỔ SUNG (NẾU CÓ)---\n{contextData}\n\n---CÂU HỎI CỦA NGƯỜI DÙNG---\n{message}";

                    var json = new JObject
                    {
                        // XÓA BỎ ["systemInstruction"] VÌ API v1 KHÔNG HỖ TRỢ

                        ["contents"] = new JArray(
                            new JObject
                            {
                                ["role"] = "user",
                                ["parts"] = new JArray(
                                    // Gửi toàn bộ nội dung đã gộp
                                    new JObject { ["text"] = fullUserMessage }
                                )
                            }
                        )
                    };

                    // (Phần còn lại của code giữ nguyên)

                    var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        return Json(new { reply = $"❌ Lỗi API: {response.StatusCode} - {error}" });
                    }

                    var responseText = await response.Content.ReadAsStringAsync();
                    var obj = JObject.Parse(responseText);

                    var reply = obj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    if (string.IsNullOrEmpty(reply))
                        reply = "❌ Không nhận được phản hồi từ trợ lý ảo.";

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