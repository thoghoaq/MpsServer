using MediatR;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.ProductBrand
{
    public class GetModels
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int BrandId { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
        }

        public class Result
        {
            public required List<Domain.Entities.ProductModel> Models { get; set; }
        }

        public class Handler(MpsDbContext context) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.ProductModels.AsEnumerable()
                    .Where(s => s.BrandId == request.BrandId)
                    .Where(s => request.Filter == null || s.Name.SearchIgnoreCase(request.Filter));

                if (request.PageNumber.HasValue && request.PageSize.HasValue)
                {
                    query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                }
                var models = query
                    .OrderBy(s => s.Name).ToList();
                return CommandResult<Result>.Success(new Result { Models = models });
            }
        }
    }
}
