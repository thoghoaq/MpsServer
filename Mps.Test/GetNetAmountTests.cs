using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Mps.Domain.Entities;
using Mps.Infrastructure.Dependencies.Setting;

namespace Mps.Test
{
    public class GetNetAmountTests
    {
        private MpsDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<MpsDbContext>()
             .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
            var context = new MpsDbContext(options);
            context.Settings.AddRange(
                new Setting { Key = "DISCOUNT_0_10", Value = "0" },
                new Setting { Key = "DISCOUNT_10_30", Value = "5" },
                new Setting { Key = "DISCOUNT_30_50", Value = "10" },
                new Setting { Key = "DISCOUNT_50_100", Value = "15" },
                new Setting { Key = "DISCOUNT_100_MAX", Value = "20" }
                );
            context.SaveChanges();
            return context;
        }

        public GetNetAmountTests()
        {
            _context = GetDbContext();
        }
        private readonly MpsDbContext _context;

        [Theory]
        [InlineData(100000)]
        [InlineData(14320000)]
        [InlineData(35000000)]
        public void SettingService_GetNetAmount_Should_ReturnValue(decimal gross)
        {
            var settingService = new SettingService(_context);
            var net = settingService.GetNetAmount(gross);
            net.Should().BeGreaterThan(0);
        }
    }
}