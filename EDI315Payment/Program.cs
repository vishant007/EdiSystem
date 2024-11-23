using EDI315Payment.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddSingleton<CosmosService>();
builder.Services.AddSingleton<AzureServiceBusService>();

// Build application
var app = builder.Build();

// Configure endpoints
app.MapControllers();

app.Run();
