using Common.Classes;
using Common.Services;
using Db;
using Microsoft.EntityFrameworkCore;
using Spam;

var builder = WebApplication.CreateBuilder(args);

const string s = "CONNECTION_STRING";
var connection = Environment.GetEnvironmentVariable(s) ?? throw new NullReferenceException(s);
builder.Services.AddDbContextPool<DatabaseContext>(options => options.UseNpgsql(connection));

builder.Services.AddSingleton<Leyla, Bot>();
builder.Services.AddHostedService<BotService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();