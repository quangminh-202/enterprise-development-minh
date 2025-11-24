using Polyclinic.Generator.Nats.Host.Interfaces;
using Polyclinic.Generator.Nats.Host.Services;
using Polyclinic.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNatsClient("polyclinic-nats");

// Add services
builder.Services.AddSingleton<IProducerService, NatsProducerService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Redirect root to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
