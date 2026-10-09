using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application;
using TaskManagement.Core;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly WeatherForecastService _service;

    public WeatherForecastController(WeatherForecastService service)
    {
        _service = service;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return _service.GetWeather();
    }
}
