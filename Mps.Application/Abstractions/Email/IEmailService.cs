using Mps.Domain.Enums;

namespace Mps.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmails, string subject, EmailTemplate template, Dictionary<string, object> parameters, Language language = Language.English);
    }
}
