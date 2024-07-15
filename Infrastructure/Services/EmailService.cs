using Core.Interfaces;
using System.Net.Mail;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using (var client = new SmtpClient("smtp.your-email-provider.com"))
            {
                var mailMessage = new MailMessage("from@example.com", to, subject, body);
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
