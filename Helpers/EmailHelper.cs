using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RunningApplicationNew.Helpers
{

    public class EmailHelper : IEmailHelper
    {
        private readonly string _smtpHost = "smtp.mail.yahoo.com"; // SMTP sunucusu (örnek olarak Gmail)
        private readonly int _smtpPort = 587; // SMTP portu
        private readonly string _smtpUser = "movliqmovliq@yahoo.com"; // SMTP kullanıcı adı
        private readonly string _smtpPassword = "AliArda12."; // SMTP şifresi
        private readonly string _fromAddress = "movliqmovliq@yahoo.com"; // Gönderen adresi

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                EnableSsl = true,
                Timeout = 1000
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
          
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Hata loglama veya daha fazla işlem
                Console.WriteLine($"E-posta gönderme hatası: {ex.Message}");
            }
        }
    }

}
