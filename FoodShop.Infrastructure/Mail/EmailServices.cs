using FoodShop.Application.Contract.Infrastructure;
using FoodShop.Application.Models.Mail;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace FoodShop.Infrastructure.Mail
{
    public class EmailServices : IEmailService
    {
        public Smtp _emailSettings { get; }

        public EmailServices(IOptions<Smtp> mailSettings)
        {
            _emailSettings = mailSettings.Value;
        }

        public async Task SendEmail(Email email)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.Server, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage(_emailSettings.SenderEmail, email.To)
                {
                    Subject = email.Subject,
                    Body = email.Body,
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                // Log detailed error information
                Console.WriteLine($"SMTP Exception: {ex.Message}");
                Console.WriteLine($"Status Code: {ex.StatusCode}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}
