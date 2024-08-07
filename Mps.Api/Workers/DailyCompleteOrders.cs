using MediatR;

namespace Mps.Api.Workers
{
    public class DailyCompleteOrders(IMediator mediator)
    {
        public async Task Process()
        {
            var result = await mediator.Send(new Application.Features.Order.DailyCompleteOrders.Command());
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.FailureReason);
            }
        }
    }
}
