using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Email;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mps.Application.Features.Account
{
    public class SendPasswordResetEmail
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            [EmailAddress]
            public required string Email { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(IEmailService emailService, IAppLocalizer localizer, MpsDbContext context, ILogger<SendPasswordResetEmail> logger, IAuthenticationService authenticationService) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly IEmailService _emailService = emailService;
            private readonly IAuthenticationService _authenticationService = authenticationService;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly MpsDbContext _context = context;
            private readonly ILogger<SendPasswordResetEmail> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
                    if (user == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["User not found"]);
                    }

                    var link = await _authenticationService.GeneratePasswordResetLinkAsync(user.Email);
                    var subject = "[SMPS System] Reset Password";
                    var currentCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
                    var language = currentCulture switch
                    {
                        "vi-VN" => Language.Vietnamese,
                        _ => Language.English
                    };
                    var parameters = new Dictionary<string, object>
                    {
                        { "Name", user.FullName },
                        { "ResetLink", link }
                    };

                    await _emailService.SendEmailAsync(user.Email, subject, EmailTemplate.PasswordReset, parameters, language);

                    return CommandResult<Result>.Success(new Result
                    {
                        Message = _localizer["Password reset email sent"]
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SendPasswordResetEmailFailure");
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }
        }
    }
}
