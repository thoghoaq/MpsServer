using MediatR;
using Mps.Application.Features.Payment;

namespace Mps.Api.Workers
{
    public class MonthlyRequestPayout(IMediator mediator)
    {
        public async Task Process()
        {
            var result = await mediator.Send(new RequestMonthlyPayout.Command { MonthToDate = DateTime.UtcNow.AddMonths(-1) });
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.FailureReason);
            }
        }
    }
}
