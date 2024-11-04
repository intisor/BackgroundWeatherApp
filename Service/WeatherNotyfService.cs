using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherNotif.Service
{
    public class WeatherNotyfService : BackgroundService
    {
        private readonly ILogger<WeatherNotyfService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public WeatherNotyfService(IHttpClientFactory httpClientFactory, ILogger<WeatherNotyfService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
             while (!stoppingToken.IsCancellationRequested)
            {
                await CheckWeatherAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); 
            }
        }

        public async Task CheckWeatherAsync(CancellationToken cancellationToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var requestUrl = $"https://api.openweathermap.org/data/2.5/weather?q=Nigeria&appid=074289fb8e5f67f4a8d04feaa291039e";

                var response = await client.GetAsync(requestUrl, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                     Console.WriteLine($"Response JSON: {json}"); // Log the raw JSON response

                    var weatherData = JsonSerializer.Deserialize<WeatherData>(json);

                    if (weatherData != null )
                    {
                        foreach (var condition in weatherData.weather)
                        {
                            Console.WriteLine($"Main: {condition.main}, Description: {condition.description}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch weather data. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    public class WeatherData
    {
        public List<WeatherCondition> weather { get; set; } 
    }

    public class WeatherCondition
    {
        public string main { get; set; }
        public string description { get; set; }
    }
}