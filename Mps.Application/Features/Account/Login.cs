using MediatR;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using System.ComponentModel.DataAnnotations;

namespace Mps.Application.Features.Account
{
    public class Login
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            [EmailAddress]
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        public class Result
        {
            public required string AccessToken { get; set; }
            public required string RefreshToken { get; set; }
            public string? ExpiresIn { get; set; }
            public GetUser.User? User { get; set; }
        }

        public class Handler(IJwtProvider jwtProvider, IMediator mediator, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly IJwtProvider _jwtProvider = jwtProvider;
            private readonly IMediator _mediator = mediator;
            private readonly IAppLocalizer _localizer = localizer;
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = await _jwtProvider.GenerateTokenAsync(request.Email, request.Password, cancellationToken);
                if (string.IsNullOrEmpty(response?.IdToken))
                {
                    return CommandResult<Result>.Fail(_localizer["Wrong email or password"]);
                }
                var commandResult = _mediator.Send(new GetUser.Query { Email = request.Email }, cancellationToken);
                return CommandResult<Result>.Success(new Result
                {
                    AccessToken = response!.IdToken,
                    RefreshToken = response.RefreshToken!,
                    ExpiresIn = response.ExpiresIn,
                    User = commandResult.Result.Payload?.User
                });
            }
        }
    }
}
