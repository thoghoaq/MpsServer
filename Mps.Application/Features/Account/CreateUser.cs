using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Application.Helpers;
using Mps.Application.Validations;
using Mps.Domain.Entities;
using Mps.Domain.Enums;
using Mps.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Mps.Application.Features.Account
{
    public class CreateUser
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            [EmailAddress]
            public required string Email { get; set; }
            [MinLength(8)]
            public string? Password { get; set; }
            [MinLength(3), FullName]
            public required string FullName { get; set; }
            [AllowedValues("Admin", "Staff", "ShopOwner", "Customer")]
            public required string Role { get; set; }
            [Phone]
            public string? PhoneNumber { get; set; }
            public string? AvatarPath { get; set; }
            public StaffData? StaffData { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public record StaffData
        {
            public string? IdentityCard { get; set; }
            public string? IdentityCardFrontPath { get; set; }
            public string? IdentityCardBackPath { get; set; }
            public string? Address { get; set; }
            public string? CertificatePath { get; set; }
        }

        public class Handler(IAuthenticationService authenticationService, MpsDbContext context, ILoggedUser loggedUser, ILogger<CreateUser> logger, IAppLocalizer localizer, IMediator mediator) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly IAuthenticationService _authenticationService = authenticationService;
            private readonly MpsDbContext _context = context;
            private readonly ILogger<CreateUser> _logger = logger;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly IMediator _mediator = mediator;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!request.Role.InRoles())
                    {
                        return CommandResult<Result>.Fail(_localizer["Role is not valid"]);
                    }
                    if (request.Role == Role.Admin.GetDescription() && !_loggedUser.Roles.Contains(Role.SuperAdmin.GetDescription()))
                    {
                        return CommandResult<Result>.Fail(_localizer["You don't have permission to create this role"]);
                    }
                    if (request.Role == Role.Staff.GetDescription() && !_loggedUser.IsAdminGroup)
                    {
                        return CommandResult<Result>.Fail(_localizer["You don't have permission to create this role"]);
                    }
                    var existUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
                    if (existUser != null)
                    {
                        return await AppendNewRole(existUser, request, cancellationToken);
                    }
                    return await CreateNewUser(request, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateUserFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }

            private async Task<CommandResult<Result>> AppendNewRole(User user, Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (user.Role.Contains(request.Role))
                    {
                        return CommandResult<Result>.Fail(_localizer["Email is existed"]);
                    }
                    user.Role = user.Role + request.Role + ",";
                    user.UpdatedAt = DateTime.UtcNow;
                    CreateRoleData(user, request);
                    await _context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Successfully created new role"] });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AppendNewRoleFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }

            private async Task<CommandResult<Result>> CreateNewUser(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (string.IsNullOrEmpty(request.Password))
                    {
                        request.Password = GeneratePassword();
                    }
                    var identityId = await _authenticationService.RegisterAsync(request.Email, request.Password, cancellationToken);
                    if (string.IsNullOrEmpty(identityId))
                    {
                        return CommandResult<Result>.Fail(_localizer["Firebase register return null"]);
                    }
                    var user = new User
                    {
                        Email = request.Email,
                        IdentityId = identityId,
                        Role = request.Role + ",",
                        FullName = request.FullName,
                        PhoneNumber = request.PhoneNumber,
                        AvatarPath = request.AvatarPath,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    CreateRoleData(user, request);
                    _context.Add(user);
                    await _context.SaveChangesAsync(cancellationToken);

                    if (request.Role == Role.Staff.GetDescription())
                    {
                        await _mediator.Send(new SendPasswordResetEmail.Command
                        {
                            Email = request.Email
                        });
                    }

                    return CommandResult<Result>.Success(new Result { Message = _localizer["Successfully created new user"] });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateNewUserFailure");
                    if (ex is not FirebaseAuthException)
                    {
                        await _authenticationService.DeleteAccountAsync(request.Email, cancellationToken);
                    }
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }

            private void CreateRoleData(User user, Command request)
            {
                if (request.Role == Role.Staff.GetDescription())
                {
                    CreateStaffData(user, request);
                }
                if (request.Role == Role.ShopOwner.GetDescription())
                {
                    CreateShopOwnerData(user, request);
                }
                if (request.Role == Role.Customer.GetDescription())
                {
                    CreateCustomerData(user, request);
                }
            }

            private void CreateStaffData(User user, Command request)
            {
                var staff = new Domain.Entities.Staff
                {
                    IdentityCard = request.StaffData?.IdentityCard,
                    IdentityCardFrontPath = request.StaffData?.IdentityCardFrontPath,
                    IdentityCardBackPath = request.StaffData?.IdentityCardBackPath,
                    Address = request.StaffData?.Address,
                    CertificatePath = request.StaffData?.CertificatePath,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                user.Staff = staff;
            }

            private void CreateShopOwnerData(User user, Command request)
            {
                var shopOwner = new ShopOwner
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                user.ShopOwner = shopOwner;
            }

            private void CreateCustomerData(User user, Command request)
            {
                var customer = new Customer
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                user.Customer = customer;
            }

            private string GeneratePassword()
            {
                var random = new Random();
                return random.Next(100000, 999999).ToString();
            }
        }
    }
}
