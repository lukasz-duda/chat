using System.ComponentModel;
using ModelContextProtocol.Server;

namespace ExchangeRate.McpServer;

[McpServerToolType]
public class ExchangeRate
{
    [McpServerTool, Description("Get currency exchange rate.")]
    public ExchangeRateResult GetCurrencyExchangeRate(
        [Description("The base currency code.")] string baseCurrency,
        [Description("The target currency code.")] string targetCurrency,
        ILogger<ExchangeRate> logger)
    {
        logger.LogInformation("Getting exchange rate from {baseCurrency} to {targetCurrency}", baseCurrency, targetCurrency);
        var normalizedBaseCurrency = baseCurrency.ToUpperInvariant();
        var normalizedTargetCurrency = targetCurrency.ToUpperInvariant();
        var rate = (normalizedBaseCurrency, normalizedTargetCurrency) switch
        {
            ("USD", "EUR") => 0.92m,
            ("EUR", "USD") => 1.09m,
            ("USD", "PLN") => 4.00m,
            ("PLN", "USD") => 0.25m,
            _ => 1.0m
        };

        return new ExchangeRateResult
        {
            Success = true,
            BaseCurrency = normalizedBaseCurrency,
            TargetCurrency = normalizedTargetCurrency,
            Rate = rate
        };
    }
}

public sealed record ExchangeRateResult
{
    [Description("Indicates whether the exchange-rate lookup succeeded.")]
    public bool Success { get; init; }

    [Description("The base currency code.")]
    public string BaseCurrency { get; init; } = string.Empty;

    [Description("The target currency code.")]
    public string TargetCurrency { get; init; } = string.Empty;

    [Description("The exchange rate from the base currency to the target currency.")]
    public decimal Rate { get; init; }
}