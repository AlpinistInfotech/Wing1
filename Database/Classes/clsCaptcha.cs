using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;

namespace Database.Classes
{
    public interface ICaptchaGenratorBase
    {
        tblCaptcha getCaptcha(int UserId = 0);
        string randomString(int length);
        byte[] textToImageConversion(string Text);
        bool verifyCaptch(string SaltId, string CaptchaCode);
    }

    public class CaptchaGenratorBase : ICaptchaGenratorBase
    {
        private readonly DBContext _context;
        public CaptchaGenratorBase(DBContext context)
        {
            _context = context;
        }
        public tblCaptcha getCaptcha(int UserId = 0)
        {
            tblCaptcha tc = new tblCaptcha()
            {
                SaltId = Convert.ToString(Guid.NewGuid()),
                CaptchaCode = randomString(4),
                CreatedDt = DateTime.Now,
                CreatedBy = UserId
            };
            _context.tblCaptcha.Add(tc);
            _context.SaveChanges();
            return tc;
        }

        public bool verifyCaptch(string SaltId, string CaptchaCode)
        {
            var Captcha = _context.tblCaptcha.Where(p => p.SaltId == SaltId && p.CaptchaCode == CaptchaCode).FirstOrDefault();
            if (Captcha != null)
            {
                _context.tblCaptcha.Remove(Captcha);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public byte[] textToImageConversion(string Text)
        {
            Bitmap bitmap = new Bitmap(1, 1);
            Font font = new Font("Arial", 25, FontStyle.Regular, GraphicsUnit.Pixel);
            Graphics graphics = Graphics.FromImage(bitmap);
            int width = (int)graphics.MeasureString(Text, font).Width;
            int height = (int)graphics.MeasureString(Text, font).Height;
            bitmap = new Bitmap(bitmap, new Size(width, height));
            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.DrawString(Text, font, new SolidBrush(Color.FromArgb(255, 0, 0)), 0, 0);
            graphics.Flush();
            graphics.Dispose();
            Byte[] data;
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Jpeg);
                data = memoryStream.ToArray();
            }
            return data;

        }
        private Random random = new Random();
        public string randomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
