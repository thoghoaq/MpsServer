using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Enums;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Account
{
    public class GetAllUsers
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public string? Role { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
            public bool? IsActive { get; set; }
        }

        public class Result
        {
            public required List<User> Users { get; set; }
        }

        public class QueryHandler(MpsDbContext context) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Users
                    .Where(u => request.IsActive == null || u.IsActive == request.IsActive)
                    .Where(u => request.Role == null || u.Role.Contains(request.Role))
                    .Where(u => request.Filter == null 
                            || u.FullName.Contains(request.Filter) 
                            || u.Email.Contains(request.Filter) 
                            || u.Role.Contains(request.Filter)
                            || (u.PhoneNumber != null && u.PhoneNumber.Contains(request.Filter))
                        )
                    .AsQueryable();

                if (request.Role == Role.Staff.GetDescription())
                {
                    query = query.Include(u => u.Staff);
                }
                if (request.Role == Role.ShopOwner.GetDescription())
                {
                    query = query.Include(u => u.ShopOwner);
                }
                if (request.Role == Role.Customer.GetDescription())
                {
                    query = query.Include(u => u.Customer);
                }
                if (request.PageNumber.HasValue && request.PageSize.HasValue)
                {
                    query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                }
                var users = await query
                    .OrderByDescending(u => u.UpdatedAt)
                    .ToListAsync(cancellationToken: cancellationToken);
                return CommandResult<Result>.Success(new Result { Users = users });
            }
        }
    }
}
