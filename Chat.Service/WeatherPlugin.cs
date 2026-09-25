using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Chat.Service;

public class WeatherPlugin
{
    [KernelFunction, Description("Gets the current weather for a specified city.")]
    public string GetWeather(
        [Description("The city name, e.g. Warsaw")] string city,
        [FromKernelServices] ILogger<WeatherPlugin> logger)
    {
        logger.LogInformation("Getting weather for city: {City}", city);
        switch (city.ToLower())
        {
            case "warsaw":
            case "warszawa":
                return "15°C, Light Rain";
            default:
                return "20°C, Sunny";
        }
    }
}