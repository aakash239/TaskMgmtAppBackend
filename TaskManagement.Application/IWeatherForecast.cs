using TaskManagement.Core;

namespace TaskManagement.Application;

public interface IWeatherForecast
{
    public IEnumerable<WeatherForecast> DisplayForecast();
}