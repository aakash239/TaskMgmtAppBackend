using TaskManagement.Core;

namespace TaskManagement.Application;

public class WeatherForecastService 
{
    private readonly IWeatherForecast _provider;
    public WeatherForecastService (IWeatherForecast provider)
    {
        _provider = provider;
    }


    public IEnumerable<WeatherForecast> GetWeather()
    {
         return _provider.DisplayForecast();
    }
}
