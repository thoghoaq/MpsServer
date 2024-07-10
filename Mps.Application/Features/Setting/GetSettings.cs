using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Setting
{
    public class GetSettings
    {
        public class Query : IRequest<CommandResult<Result>>
        {
        }

        public class Result
        {
            public List<Domain.Entities.Setting> Settings { get; set; } = [];
        }

        public class Handler(MpsDbContext context) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var settings = await _context.Settings.ToListAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Settings = settings });
                }
                catch (Exception ex)
                {
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
