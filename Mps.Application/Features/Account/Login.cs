using MediatR;
using Mps.Application.Abstractions.Authentication;
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
            public required string Token { get; set; }
        }

        public class Handler(IJwtProvider jwtProvider) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly IJwtProvider _jwtProvider = jwtProvider;
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = await _jwtProvider.GenerateTokenAsync(request.Email, request.Password, cancellationToken);
                if (string.IsNullOrEmpty(response))
                {
                    return CommandResult<Result>.Fail("Invalid email or password");
                }
                return CommandResult<Result>.Success(new Result { Token = response });
            }
        }
    }
}
