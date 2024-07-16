using MediatR;
using Mps.Application.Features.Payment;
using Mps.Domain.Enums;

namespace Mps.Api.Workers
{
    public class CustomRequestPayout(IMediator mediator)
    {
        public async Task Process(PayoutDate payoutDate)
        {
            var result = await mediator.Send(new RequestCustomPayout.Command { MonthToDate = DateTime.UtcNow, PayoutDate = payoutDate });
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.FailureReason);
            }
        }
    }
}
