using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.ProductCategory
{
    public class GetCategories
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
        }

        public class Result
        {
            public required List<Domain.Entities.ProductCategory> Categories { get; set; }
        }

        public class Handler(MpsDbContext context) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.ProductCategories
                    .Include(s => s.Children)
                    .Where(s => request.Filter == null || s.Name.Contains(request.Filter))
                    .AsQueryable();

                if (request.PageNumber.HasValue && request.PageSize.HasValue)
                {
                    query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                }
                var categories = await query
                    .OrderBy(s => s.Name)
                    .ToListAsync(cancellationToken: cancellationToken);
                return CommandResult<Result>.Success(new Result { Categories = categories });
            }
        }
    }
}
