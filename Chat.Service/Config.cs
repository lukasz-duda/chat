namespace Chat.Service;

public static class ConfigurationExtensions
{
    public static OllamaOptions GetOllamaOptions(this IConfiguration configuration)
    {
        var ollamaOptions = configuration.GetSection("Ollama").Get<OllamaOptions>()
            ?? throw new InvalidOperationException("Ollama configuration is missing.");
        return ollamaOptions;
    }

    public static string GetExchangeRateApiEndpoint(this IConfiguration configuration)
    {
        var exchangeRateApi = configuration.GetSection("ExchangeRate:BaseUrl").Value
            ?? throw new InvalidOperationException("Exchange Rate API endpoint is missing.");
        return exchangeRateApi;
    }
}

public record OllamaOptions(string BaseUrl, string Model);