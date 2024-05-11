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
                if (string.IsNullOrEmpty(response))
                {
                    return CommandResult<Result>.Fail(_localizer["Invalid email or password"]);
                }
                var commandResult = _mediator.Send(new GetUser.Query { Email = request.Email }, cancellationToken);
                return CommandResult<Result>.Success(new Result { 
                    AccessToken = response,
                    User = commandResult.Result.Payload?.User
                });
            }
        }
    }
}
