using MediatR;

namespace Nexile.Desktop.Core.Internal;

public interface ICurrencyPricingService
{
    public IReadOnlyCollection<string> SubscribedCurrencies { get; }
    
    public bool AddCurrency(string currencyName);
    public bool RemoveCurrency(string currencyName);
}

public class CurrencyPriceUpdatedEvent : INotification
{
    public CurrencyPriceUpdatedEvent(string currencyName)
    {
        CurrencyName = currencyName;
    }

    public string CurrencyName { get; }
}