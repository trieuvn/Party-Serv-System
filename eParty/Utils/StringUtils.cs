using QRCoder;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace eParty.Utils
{
    public static class StringUtils
    {
        public static string ImageToBase64(string imagePath)
        {
            try
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, image.RawFormat);
                        byte[] imageBytes = ms.ToArray();
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi chuyển đổi hình ảnh sang Base64: " + ex.Message);
            }
        }

        public static string StringToQRCodeBase64(string input)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.Q);
                using (Bitmap qrCodeImage = new QRCode(qrCodeData).GetGraphic(20))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] imageBytes = ms.ToArray();
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo mã QR: " + ex.Message);
            }
        }

        public static string EncryptString(string plainText, string key = "YourSecretKey12345678901234567890")
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] array;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                            array = ms.ToArray();
                        }
                    }
                }
                return Convert.ToBase64String(array);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi mã hóa chuỗi: " + ex.Message);
            }
        }
    }
}