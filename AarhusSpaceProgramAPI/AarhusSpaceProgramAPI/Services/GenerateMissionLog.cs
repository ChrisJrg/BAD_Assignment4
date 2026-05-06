
using AarhusSpaceProgramAPI.Models;
using MongoDB.Driver;

namespace AarhusSpaceProgramAPI.Services;

public class GenerateMissionLog : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMongoDatabase _database;
    public GenerateMissionLog(IHttpClientFactory httpClientFactory,  IMongoDatabase database)
    {
        _httpClientFactory = httpClientFactory;
        _database = database;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        HttpClient client = _httpClientFactory.CreateClient();
        
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await client.GetAsync("http://localhost:5265/api/Mission?status=Active", stoppingToken);
                response.EnsureSuccessStatusCode();
                var missions = response.Content.ReadFromJsonAsync<List<MissionStatusDto>>().Result;
                var db = _database.GetCollection<MissionLog>("MissionLog");
                foreach (var mission in missions)
                {
                    var log = new MissionLog
                    {
                        MissionId = mission.MissionId.ToString(),
                        Message = "Telemetry check completed",
                        Timestamp = DateTime.Now,
                    };
                    await db.InsertOneAsync(log);
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}