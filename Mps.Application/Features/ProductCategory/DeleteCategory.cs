using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.ProductCategory
{
    public class DeleteCategory
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public int Id { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext dbContext, IAppLocalizer localizer, ILogger<DeleteCategory> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var category = await dbContext.ProductCategories.FindAsync(request.Id, cancellationToken);
                    if (category == null)
                    {
                        return CommandResult<Result>.Fail(localizer["Category not found"]);
                    }
                    category.IsDeleted = true;
                    await dbContext.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = localizer["Category deleted successfully"] });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(localizer[ex.Message]);
                }
            }
        }
    }
}
