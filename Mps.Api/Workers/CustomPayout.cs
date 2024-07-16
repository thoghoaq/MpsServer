using MediatR;
using Mps.Domain.Enums;

namespace Mps.Api.Workers
{
    public class CustomPayout(IMediator mediator)
    {
        public async Task Process(PayoutDate payoutDate)
        {
            var result = await mediator.Send(new Application.Features.Payment.CustomPayout.Command { MonthToDate = DateTime.UtcNow, PayoutDate = payoutDate });
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.FailureReason);
            }
        }
    }
}
