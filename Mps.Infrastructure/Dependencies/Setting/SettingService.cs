using Mps.Application.Abstractions.Setting;
using Mps.Domain.Entities;

namespace Mps.Infrastructure.Dependencies.Setting
{
    public class SettingService(MpsDbContext context) : ISettingService
    {
        public decimal GetNetAmount(decimal gross)
        {
            return gross * (1 - GetRate(gross));
        }

        public decimal GetNetBySetting(decimal gross, List<Domain.Entities.Setting> settings)
        {
            return gross * (1 - GetRateBySetting(gross, settings));
        }

        public decimal GetRate(decimal gross)
        {
            var settings = context.Settings.ToList();
            return GetRateBySetting(gross, settings);
        }

        public decimal GetRateBySetting(decimal gross, List<Domain.Entities.Setting> settings)
        {
            if (gross < 10000000)
            {
                return decimal.Parse(settings.First(x => x.Key == "DISCOUNT_0_10")!.Value) / 100;
            }
            else if (gross < 30000000)
            {
                return decimal.Parse(settings.First(x => x.Key == "DISCOUNT_10_30")!.Value) / 100;
            }
            else if (gross < 50000000)
            {
                return decimal.Parse(settings.First(x => x.Key == "DISCOUNT_30_50")!.Value) / 100;
            }
            else if (gross < 100000000)
            {
                return decimal.Parse(settings.First(x => x.Key == "DISCOUNT_50_100")!.Value) / 100;
            }
            else
            {
                return decimal.Parse(settings.First(x => x.Key == "DISCOUNT_100_MAX")!.Value) / 100;
            }
        }

        public decimal GetShopNetAmount(decimal gross, int shopId)
        {
            return gross * (1 - GetShopRate(gross, shopId));
        }

        public decimal GetShopRate(decimal gross, int shopId)
        {
            var settings = context.ShopSettings.Where(x => x.ShopId == shopId)
                .Select(x => new Domain.Entities.Setting
                {
                    Key = x.Key,
                    Value = x.Value
                })
                .ToList();
            return GetRateBySetting(gross, settings);
        }
    }
}
