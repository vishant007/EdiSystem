var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CosmosService>(sp =>
    new CosmosService(
        builder.Configuration["CosmosDb:Account"],
        builder.Configuration["CosmosDb:Key"],
        builder.Configuration["CosmosDb:DatabaseName"],
        builder.Configuration["CosmosDb:ContainerName"]
    ));

builder.Services.AddSingleton<PaymentService>();

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

app.Run();
