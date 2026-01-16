using Microsoft.AspNetCore.Mvc;

namespace Odasoft.XBOL.AdminAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(ITicketingClient ticketingClient) : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var result = await ticketingClient.GetTestAsync();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
