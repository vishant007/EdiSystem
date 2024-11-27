using EDI315Payment.Services;
using EDI315Payment.Data;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<SqlService>(); 
builder.Services.AddSingleton<CosmosService>();
builder.Services.AddSingleton<AzureServiceBusService>();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 


var app = builder.Build();


app.UseCors("AllowAll");


app.UseSwagger(); 
app.UseSwaggerUI(); 


app.MapControllers();


app.Run();
