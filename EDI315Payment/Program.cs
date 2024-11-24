using EDI315Payment.Services;
using EDI315Payment.Data;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register PaymentDbContext with the SQL Server connection
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<SqlService>(); // Register SqlService as scoped
builder.Services.AddSingleton<CosmosService>();
builder.Services.AddSingleton<AzureServiceBusService>();
builder.Services.AddControllers();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Swagger configuration
builder.Services.AddEndpointsApiExplorer(); // Required to expose endpoints to Swagger
builder.Services.AddSwaggerGen(); // Adds Swagger generation services

// Build application
var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Use Swagger
app.UseSwagger(); // Generates Swagger JSON file
app.UseSwaggerUI(); // Provides a user-friendly Swagger UI

// Configure endpoints
app.MapControllers();

// Run application
app.Run();
