using Microsoft.Extensions.Configuration;
using Mps.Application.Abstractions.Email;
using Mps.Domain.Enums;
using System.Net;
using System.Net.Mail;

namespace Mps.Infrastructure.Dependencies.Email
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task SendEmailAsync(string toEmails, string subject, EmailTemplate template, Dictionary<string, object> parameters, Language language = Language.English)
        {
            string host = _configuration["Email:Smtp:Host"]!;
            int port = int.Parse(_configuration["Email:Smtp:Port"]!);
            string fromEmail = _configuration["Email:Smtp:FromEmail"]!;
            string password = _configuration["Email:Smtp:Password"]!;

            var body = await ReadTemplateFileAsync(template, parameters, language);
            var message = new MailMessage()
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            foreach (var email in toEmails.Split(","))
            {
                message.To.Add(email);
            }

            var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true,
            };

            smtp.Send(message);
        }

        private async Task<string> ReadTemplateFileAsync(EmailTemplate template, Dictionary<string, object> parameters, Language language)
        {
            var fileName = template switch
            {
                EmailTemplate.PasswordReset => "PasswordReset.html",
                _ => throw new ArgumentOutOfRangeException(nameof(template), template, null)
            };

            var path = language switch
            {
                Language.English => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Dependencies\Email\Templates", "En", fileName),
                Language.Vietnamese => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Dependencies\Email\Templates", "Vi", fileName),
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
            };

            var content = await File.ReadAllTextAsync(path);
            return BuildEmailContent(content, parameters);
        }

        private string BuildEmailContent(string content, Dictionary<string, object> parameters)
        {
            foreach (var param in parameters)
            {
                content = content.Replace($"{{{{{param.Key}}}}}", param.Value.ToString());
            }
            return content;
        }
    }
}
