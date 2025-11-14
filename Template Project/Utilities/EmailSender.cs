using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace ECommerce.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("hussienoasiscs@gmail.com", "item zrxr putc otxc ")
            };


            return client.SendMailAsync(
                new MailMessage(from: "hussienoasiscs@gmail.com",
                to:email,
                subject,
                htmlMessage)
                {
                    IsBodyHtml = true
                });
        }
    }
}
