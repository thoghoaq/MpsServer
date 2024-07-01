﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Payment
{
    public class RequestPayout
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int ShopId { get; set; }
            public required DateTime MonthToDate { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILoggedUser loggedUser, ILogger<RequestPayout> logger, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!loggedUser.IsShopOwnerOf(request.ShopId))
                    {
                        return CommandResult<Result>.Fail(localizer["Unauthorized"]);
                    }

                    var payout = await dbContext.Payouts.FirstOrDefaultAsync(p => p.ShopId == request.ShopId
                    && p.MonthToDate.Month == request.MonthToDate.Month
                    && p.MonthToDate.Year == request.MonthToDate.Year
                    && p.PayoutStatusId != (int)Domain.Enums.PayoutStatus.Failed
                    , cancellationToken);
                    if (payout != null)
                    {
                        return CommandResult<Result>.Fail(localizer["Payout request has been sent"]);
                    }

                    dbContext.Payouts.Add(new Payout
                    {
                        ShopId = request.ShopId,
                        MonthToDate = request.MonthToDate,
                        CreatedDate = DateTime.UtcNow,
                        PayoutStatusId = (int)Domain.Enums.PayoutStatus.Pending
                    });

                    await dbContext.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = localizer["Payout request has been sent"] });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(localizer["An error occurred"]);
                }
            }
        }
    }
}