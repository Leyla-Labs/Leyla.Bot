using Common.Db;
using Common.Enums;
using Common.Services;
using Main;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string s = "CONNECTION_STRING";
var connection = Environment.GetEnvironmentVariable(s) ?? throw new NullReferenceException(s);
builder.Services.AddDbContextPool<DatabaseContext>(options => options.UseNpgsql(connection));

const string m = "MODULES";
var modulesStr = Environment.GetEnvironmentVariable(m)?.Split(";") ?? throw new NullReferenceException(m);
var modules = modulesStr.Select(x => (LeylaModule) Enum.Parse(typeof(LeylaModule), x, true)).ToArray();

if (modules.Contains(LeylaModule.Main))
{
    builder.Services.AddSingleton<Bot>();
    builder.Services.AddHostedService<BotService<Bot>>();
}

if (modules.Contains(LeylaModule.Logs))
{
    builder.Services.AddSingleton<Logs.Bot>();
    builder.Services.AddHostedService<BotService<Logs.Bot>>();
}

if (modules.Contains(LeylaModule.Spam))
{
    builder.Services.AddSingleton<Spam.Bot>();
    builder.Services.AddHostedService<BotService<Spam.Bot>>();
}

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();