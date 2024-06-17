using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Payment
{
    public class GetPaymentMethods
    {
        public class Query : IRequest<CommandResult<List<Result>>>
        {
        }

        public class Result
        {
            public int Id { get; set; }
            public required string Name { get; set; }
        }

        public class Handler(MpsDbContext context) : IRequestHandler<Query, CommandResult<List<Result>>>
        {
            public async Task<CommandResult<List<Result>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = await context.PaymentMethods.Select(x => new Result
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync(cancellationToken);
                return CommandResult<List<Result>>.Success(result);
            }
        }
    }
}
