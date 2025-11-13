using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Transaction.Infrastructure.Kafka;
using Transaction.Infrastructure.Repositories;

namespace Transaction.Infrastructure;
public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TransactionDbContext>(options =>
        options.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsAssembly(typeof(TransactionDbContext).Assembly.FullName))
        );
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IEventProducer, KafkaProducer>();

        return services;
    }
}