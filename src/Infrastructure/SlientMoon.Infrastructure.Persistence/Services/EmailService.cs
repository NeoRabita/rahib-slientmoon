using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SlientMoon.Application.DTOs.Email;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Infrastructure.Persistence.Settings;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class EmailService : IEmailService
    {
        public APIAppSettings _apiSettings;

        public EmailService(IOptions<APIAppSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        public async Task SendAsync(EmailRequest request)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress(request.From ?? _apiSettings.MailSettings.EmailFrom, _apiSettings.MailSettings.DisplayName);
                mail.To.Add(request.To);
                mail.Subject = request.Subject;
                var htmlView = AlternateView.CreateAlternateViewFromString(request.Body, null, "text/html");
                mail.IsBodyHtml = true;
                mail.AlternateViews.Add(htmlView);
                using (var smtpClient = new SmtpClient(_apiSettings.MailSettings.SmtpHost, _apiSettings.MailSettings.SmtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_apiSettings.MailSettings.SmtpUser, _apiSettings.MailSettings.SmtpPass);
                    smtpClient.EnableSsl = _apiSettings.MailSettings.SSL;
                    await smtpClient.SendMailAsync(mail);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            // Gözəl və sadə bir HTML şablonu hazırlayırıq
            string htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
                    <h2 style='color: #333; text-align: center;'>SilentMoon Təsdiqləmə Kodu</h2>
                    <p>Salam,</p>
                    <p>SilentMoon platformasında qeydiyyatınızı tamamlamaq üçün birdəfəlik təsdiqləmə (OTP) kodunuz aşağıdadır:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                         <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #4A90E2; background-color: #f5f7fa; padding: 10px 30px; border-radius: 5px; border: 1px dashed #4A90E2;'>
                            {otpCode}
                           </span>
                    </div>
                    <p style='color: #666; font-size: 12px;'>Bu kod <b>10 dəqiqə</b> ərzində qüvvədədir. Əgər bu istəyi siz etməmisinizsə, bu məktubu görməzdən gələ bilərsiniz.</p>
                        <hr style='border: none; border-top: 1px solid #e0e0e0; margin-top: 30px;'>
                    <p style='text-align: center; color: #999; font-size: 12px;'>© 2026 SilentMoon API. Bütün hüquqlar qorunur.</p>
                </div>";

            var emailRequest = new EmailRequest
            {
                To = toEmail,
                Subject = "SilentMoon - E-poçt Təsdiqləmə Kodu",
                Body = htmlBody
            };

            await SendAsync(emailRequest);
        }
    }
}