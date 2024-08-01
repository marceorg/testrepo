using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TestRepo.Model;
using TestRepo.Services;

namespace TestRepo.Controllers
{
    public class Settings
    {
        public string BackgroundColor { get; set; }
        public long FontSize { get; set; }
        public string FontColor { get; set; }
        public string Message { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherController> _logger;
        private readonly IConfiguration _config;
        private readonly Settings _settings;
        private readonly ApplicationDbContext _dbContext;

        public WeatherController(ILogger<WeatherController> logger, 
                        IConfiguration config, 
                        IOptionsSnapshot<Settings> options,
                        ApplicationDbContext dbContext
            )
        {
            _logger = logger;
            _config = config;
            _settings = options.Value;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("db")]
        public IActionResult GetData()
        {
            var n = _dbContext.Database.GetDbConnection().ConnectionString;
            //var e = _dbContext.Database.CanConnect();
            return Ok(n);
        }

        [HttpGet("settings")]
        public IActionResult GetSettings()
        {
        //    var conn = _config["Db:ConnectionString"];
        //    return Ok(conn);
            return Ok(_settings);
        }

        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok("1.0.1");
        }
    }
}
