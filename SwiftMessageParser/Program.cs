using NLog.Web;
using SwiftMessageParser.Business;
using SwiftMessageParser.Business.Contracts;
using SwiftMessageParser.Data;
using SwiftMessageParser.Data.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IParser, Parser>();
builder.Services.AddScoped<ISwiftMessageRepository, SwiftMessageRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();

builder.Host.UseNLog();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
var databaseInitializer = new DatabaseInitializer(builder.Configuration);

databaseInitializer.SetUp();

app.Run();