using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Transaction.Aplication.Common.Kakfa;
using Transaction.Aplication.Interfaces;
using Transaction.Domain.Interfaces;
using Transaction.Infrastructure.Kafka;
using Transaction.Infrastructure.Repositories;
using Transaction.Infrastructure.TransactionsDbContext;
using Transaction.Msvc.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly(typeof(TransactionDbContext).Assembly.FullName)
    )
);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.Load("Transaction.Aplication")));

builder.Services.AddSingleton<ProducerConfig>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    };
    return config;
});

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IEventProducer, KafkaProducer>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
