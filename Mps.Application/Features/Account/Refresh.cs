using MediatR;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;

namespace Mps.Application.Features.Account
{
    public class Refresh
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required string RefreshToken { get; set; }
        }

        public class Result
        {
            public required string AccessToken { get; set; }
            public required string RefreshToken { get; set; }
            public string? ExpiresIn { get; set; }
        }

        public class Handler(IJwtProvider jwtProvider, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = await jwtProvider.RefreshTokenAsync(request.RefreshToken, cancellationToken);
                if (string.IsNullOrEmpty(response?.IdToken))
                {
                    return CommandResult<Result>.Fail(localizer["Invalid refresh token"]);
                }
                return CommandResult<Result>.Success(new Result
                {
                    AccessToken = response!.IdToken,
                    RefreshToken = response.RefreshToken!,
                    ExpiresIn = response.ExpiresIn
                });
            }
        }
    }
}
