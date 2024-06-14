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
                var matches = query.AsEnumerable()
                    .Where(u => request.Role == null || u.Role.SearchIgnoreCase(request.Role))
                    .Where(u => request.Filter == null
                            || u.FullName.SearchIgnoreCase(request.Filter)
                            || u.Email.SearchIgnoreCase(request.Filter)
                            || u.Role.SearchIgnoreCase(request.Filter)
                            || (u.PhoneNumber != null && u.PhoneNumber.SearchIgnoreCase(request.Filter))
                            || (u.Staff != null && u.Staff.StaffCode.SearchIgnoreCase(request.Filter))
                        )
                    .ToList();

                if (request.PageNumber.HasValue && request.PageSize.HasValue)
                {
                    matches = matches.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value).ToList();
                }

                var users = matches.OrderByDescending(u => u.UpdatedAt).ToList();
                return CommandResult<Result>.Success(new Result { Users = users });
            }
        }
    }
}
