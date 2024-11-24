using EDI315Payment.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddSingleton<CosmosService>();
builder.Services.AddSingleton<AzureServiceBusService>();
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });
builder.Services.AddEndpointsApiExplorer(); // Required to expose endpoints to Swagger
builder.Services.AddSwaggerGen(); // Adds Swagger generation services


// Build application
var app = builder.Build();

app.UseCors("AllowAll");

 app.UseSwagger(); // Generates Swagger JSON file
 app.UseSwaggerUI(); // Provides a user-friendly Swagger UI

// Configure endpoints
app.MapControllers();


app.Run();
