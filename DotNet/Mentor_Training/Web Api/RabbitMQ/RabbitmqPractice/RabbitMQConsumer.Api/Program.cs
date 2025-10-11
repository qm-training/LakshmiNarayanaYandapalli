using Microsoft.EntityFrameworkCore;
using RabbitMQConsumer.Core.Contracts.Repository;
using RabbitMQConsumer.Core.Contracts.Services;
using RabbitMQConsumer.Core.Options;
using RabbitMQConsumer.Infrastructure.Data;
using RabbitMQConsumer.Infrastructure.Repository;
using RabbitMQConsumer.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RabbitMQContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.Configure<RabbitMQOptions>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<RabbitMQServices>();
builder.Services.AddTransient<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

builder.Services.AddHostedService<RabbitMQConsumerService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();