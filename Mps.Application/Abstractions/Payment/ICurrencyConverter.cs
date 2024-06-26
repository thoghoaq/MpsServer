namespace Mps.Application.Abstractions.Payment
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, CancellationToken cancellationToken);
        Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency, CancellationToken cancellationToken);
    }
}
