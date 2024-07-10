namespace Mps.Application.Abstractions.Setting
{
    public interface ISettingService
    {
        public decimal GetNetAmount(decimal gross);
        public decimal GetRate(decimal gross);
        public decimal GetShopNetAmount(decimal gross, int shopId);
        public decimal GetShopRate(decimal gross, int shopId);
        public decimal GetRateBySetting(decimal gross, List<Domain.Entities.Setting> settings);
        public decimal GetNetBySetting(decimal gross, List<Domain.Entities.Setting> settings);
    }
}
