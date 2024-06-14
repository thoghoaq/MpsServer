using MediatR;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.ProductBrand
{
    public class GetBrands
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
        }

        public class Result
        {
            public required List<Domain.Entities.ProductBrand> Brands { get; set; }
        }

        public class Handler(MpsDbContext context) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.ProductBrands.AsEnumerable()
                    .Where(s => request.Filter == null || s.Name.SearchIgnoreCase(request.Filter));

                if (request.PageNumber.HasValue && request.PageSize.HasValue)
                {
                    query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                }
                var brands = query
                    .OrderBy(s => s.Name).ToList();
                return CommandResult<Result>.Success(new Result { Brands = brands });
            }
        }

    }
}
