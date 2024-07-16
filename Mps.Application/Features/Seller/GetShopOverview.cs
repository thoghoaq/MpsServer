using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Abstractions.Setting;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Seller
{
    public class GetShopOverview
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public required int ShopId { get; set; }
            public required DateTime MonthToDate { get; set; }
        }

        public class Result
        {
            public required Overview Overview { get; set; }
            public required List<DailyRevenue> DailyRevenues { get; set; }
            public required List<ProductSoldByCategory> ProductSoldByCategories { get; set; }
            public required List<RecentOrder> RecentOrders { get; set; }
            public required List<TopProduct> TopProducts { get; set; }
        }

        public class Overview
        {
            public int TotalSales { get; set; }
            public double SalePercentageWithLastMonth { get; set; }
            public decimal TotalRevenue { get; set; }
            public double RevenuePercentageWithLastMonth { get; set; }
            public decimal TotalNet { get; set; }
            public double NetPercentageWithLastMonth { get; set; }
            public int TotalCustomers { get; set; }
            public double CustomerPercentageWithLastMonth { get; set; }
        }

        public class DailyRevenue
        {
            public int Date { get; set; }
            public decimal Total { get; set; }
            public decimal Net { get; set; }
        }

        public class ProductSoldByCategory
        {
            public required string CategoryName { get; set; }
            public int Total { get; set; }
        }

        public class RecentOrder
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal Total { get; set; }
            public int OrderStatus { get; set; }
        }

        public class TopProduct
        {
            public int ProductId { get; set; }
            public required string ProductName { get; set; }
            public string? ProductImage { get; set; }
            public decimal Price { get; set; }
            public int SoldCount { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILogger<GetShopOverview> logger, IAppLocalizer localizer, ILoggedUser loggedUser, ISettingService settingService) : IRequestHandler<Query, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!loggedUser.IsShopOwnerOf(request.ShopId))
                    {
                        return CommandResult<Result>.Fail(localizer["Unauthorized"]);
                    }

                    var result = new Result
                    {
                        Overview = new Overview(),
                        DailyRevenues = [],
                        ProductSoldByCategories = [],
                        RecentOrders = [],
                        TopProducts = []
                    };

                    var shopOrdersInLastTwoMonths = dbContext.Orders
                        .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                        .ThenInclude(od => od.Images)
                        .Where(o => o.ShopId == request.ShopId)
                        .AsEnumerable()
                        .Where(o => o.OrderDate.CompareMonth(request.MonthToDate) || o.OrderDate.CompareMonth(request.MonthToDate.AddMonths(-1)))
                        .ToList();

                    var shopOrdersInMonth = shopOrdersInLastTwoMonths
                        .Where(o => o.OrderDate.CompareMonth(request.MonthToDate))
                        .ToList();

                    var shopOrdersLastMonth = shopOrdersInLastTwoMonths
                        .Where(o => o.OrderDate.CompareMonth(request.MonthToDate.AddMonths(-1)))
                        .ToList();

                    result.Overview.TotalSales = shopOrdersInMonth.Sum(o => o.OrderDetails.Sum(od => od.Quantity));
                    var totalSalesLastMonth = shopOrdersLastMonth.Sum(o => o.OrderDetails.Sum(od => od.Quantity));
                    var totalSalesInMonth = shopOrdersInMonth.Sum(o => o.OrderDetails.Sum(od => od.Quantity));
                    result.Overview.SalePercentageWithLastMonth = totalSalesLastMonth == 0 ? 1 : ((double)(totalSalesInMonth - totalSalesLastMonth) / (double)totalSalesLastMonth);
                    result.Overview.TotalRevenue = shopOrdersInMonth.Sum(o => o.TotalAmount);
                    var totalRevenueLastMonth = shopOrdersLastMonth.Sum(o => o.TotalAmount);
                    var totalRevenueInMonth = shopOrdersInMonth.Sum(o => o.TotalAmount);
                    result.Overview.RevenuePercentageWithLastMonth = totalRevenueLastMonth == 0 ? 1 : (double)((totalRevenueInMonth - totalRevenueLastMonth) / totalRevenueLastMonth);
                    var settings = dbContext.Settings.ToList();
                    var totalNetLastMonth = shopOrdersLastMonth.Sum(o => settingService.GetNetBySetting(o.TotalAmount, settings));
                    var totalNetInMonth = shopOrdersInMonth.Sum(o => settingService.GetNetBySetting(o.TotalAmount, settings));
                    result.Overview.TotalNet = totalNetInMonth;
                    result.Overview.NetPercentageWithLastMonth = totalNetLastMonth == 0 ? 1 : (double)((totalNetInMonth - totalNetLastMonth) / totalNetLastMonth);
                    result.Overview.TotalCustomers = shopOrdersInMonth.Select(o => o.CustomerId).Distinct().Count();
                    var totalCustomersLastMonth = shopOrdersLastMonth.Select(o => o.CustomerId).Distinct().Count();
                    var totalCustomersInMonth = shopOrdersInMonth.Select(o => o.CustomerId).Distinct().Count();
                    result.Overview.CustomerPercentageWithLastMonth = totalCustomersLastMonth == 0 ? 1 : ((double)(totalCustomersInMonth - totalCustomersLastMonth) / (double)totalCustomersLastMonth);

                    result.DailyRevenues = Enumerable.Range(1, DateTime.DaysInMonth(request.MonthToDate.Year, request.MonthToDate.Month))
                        .Select(day => new DailyRevenue
                        {
                            Date = day,
                            Total = shopOrdersInMonth
                                .Where(o => o.OrderDate.Day == day)
                                .Sum(o => o.TotalAmount),
                            Net = shopOrdersInMonth.Where(o => o.OrderDate.Day == day).Sum(o => settingService.GetNetBySetting(o.TotalAmount, settings))
                        })
                        .ToList();

                    var allParentCategories = await dbContext.ProductCategories
                        .Where(c => c.ParentId == null)
                        .Include(c => c.Children)
                        .ThenInclude(c => c.Children)
                        .ToListAsync(cancellationToken);

                    // get all products sold in the month with category in allParentCategories, product may in child category
                    var productsSoldInMonth = shopOrdersInMonth
                        .SelectMany(o => o.OrderDetails)
                        .Select(od => od.Product)
                        .Where(p => allParentCategories.Any(c => c.Id == p.CategoryId || c.Children.Any(cc => cc.Id == p.CategoryId) || c.Children.Any(cc => cc.Children.Any(ccc => ccc.Id == p.CategoryId))))
                        .ToList();
                    result.ProductSoldByCategories = allParentCategories
                        .Select(c => new ProductSoldByCategory
                        {
                            CategoryName = c.Name,
                            Total = productsSoldInMonth.Count(p => c.Id == p.CategoryId || c.Children.Any(cc => cc.Id == p.CategoryId) || c.Children.Any(cc => cc.Children.Any(ccc => ccc.Id == p.CategoryId)))
                        })
                        .Where(p => p.Total > 0)
                        .ToList();

                    result.TopProducts = shopOrdersInMonth
                        .SelectMany(o => o.OrderDetails)
                        .GroupBy(od => od.ProductId)
                        .Select(g => new TopProduct
                        {
                            ProductId = g.Key,
                            ProductName = g.First().Product!.Name,
                            ProductImage = g.First().Product!.Images.FirstOrDefault()?.ImagePath,
                            Price = g.First().Product!.Price,
                            SoldCount = g.First().Product!.SoldCount
                        })
                        .OrderByDescending(tp => tp.SoldCount)
                        .Take(6)
                        .ToList();

                    result.RecentOrders = shopOrdersInMonth
                        .OrderByDescending(o => o.OrderDate)
                        .Take(10)
                        .Select(o => new RecentOrder
                        {
                            OrderId = o.Id,
                            OrderDate = o.OrderDate,
                            Total = o.TotalAmount,
                            OrderStatus = o.OrderStatusId,
                        })
                        .ToList();

                    return CommandResult<Result>.Success(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(localizer["An error occurred while getting shop overview"]);
                }
            }
        }
    }
}
