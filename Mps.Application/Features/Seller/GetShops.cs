using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Seller
{
    public class GetShops
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public string? Filter { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
        }

        public class Result
        {
            public required List<ShopResult> Shops { get; set; }
        }

        public class ShopResult
        {
            public int Id { get; set; }
            public int ShopOwnerId { get; set; }
            public required string ShopName { get; set; }
            public required string PhoneNumber { get; set; }
            public required string Address { get; set; }
            public required string City { get; set; }
            public required string District { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string? Description { get; set; }
            public string? Avatar { get; set; }
            public string? Cover { get; set; }
            public bool IsActive { get; set; }
            public bool IsAccepted { get; set; }

            public string? PayPalAccount { get; set; }

            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public string? Comment { get; set; }
            public int ProcessingOrderCount { get; set; }
        }

        public record ShopOrderCount
        {
            public int ShopId { get; set; }
            public int ProcessingOrderCount { get; set; }

            public class QueryHandler(MpsDbContext context, ILoggedUser loggedUser) : IRequestHandler<Query, CommandResult<Result>>
            {
                private readonly MpsDbContext _context = context;

                public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
                {
                    var query = _context.Shops
                        .Where(s => s.ShopOwnerId == loggedUser.UserId)
                        .Where(s => request.Filter == null
                            || s.ShopName.Contains(request.Filter)
                         )
                        .AsQueryable();
                    if (request.PageNumber.HasValue && request.PageSize.HasValue)
                    {
                        query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                    }
                    var shops = await query
                        .OrderByDescending(s => s.UpdatedAt)
                        .ToListAsync(cancellationToken: cancellationToken);

                    var shopOrdersProcessingCount = _context.Orders
                        .Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Processing)
                        .GroupBy(o => o.ShopId)
                        .Select(g => new ShopOrderCount
                        {
                            ShopId = g.Key,
                            ProcessingOrderCount = g.Count()
                        }).ToList();

                    return CommandResult<Result>.Success(new Result
                    {
                        Shops = shops.Select(s => new ShopResult
                        {
                            Address = s.Address,
                            Avatar = s.Avatar,
                            City = s.City,
                            Comment = s.Comment,
                            Cover = s.Cover,
                            CreatedAt = s.CreatedAt,
                            District = s.District,
                            Description = s.Description,
                            Id = s.Id,
                            IsActive = s.IsActive,
                            IsAccepted = s.IsAccepted,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            PayPalAccount = s.PayPalAccount,
                            PhoneNumber = s.PhoneNumber,
                            ShopName = s.ShopName,
                            ShopOwnerId = s.ShopOwnerId,
                            UpdatedAt = s.UpdatedAt,
                            ProcessingOrderCount = shopOrdersProcessingCount.Find(x => x.ShopId == s.Id)?.ProcessingOrderCount ?? 0
                        }).ToList()
                    });
                }
            }
        }
    }
