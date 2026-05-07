using GenerateMissionLogs;
using GenerateMissionLogs.Services;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<GenerateMissionLog>();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient("mongodb://mongodb:27017"));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("SpaceProgramLogs");
});

var host = builder.Build();
host.Run();