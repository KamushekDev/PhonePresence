using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PresenceBot.Infrastructure.Database;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();

        if (settings is null)
            throw new NotImplementedException("Кинуть нормальную ошибку конфигурации");

        Directory.CreateDirectory(settings.Path);
        var source = Path.Combine(settings.Path, settings.Filename);

        // todo: ensure it has been created
        services.AddDbContext<PresenceContext>(options => { options.UseSqlite($"Data Source={source};"); });

        return services;
    }
}