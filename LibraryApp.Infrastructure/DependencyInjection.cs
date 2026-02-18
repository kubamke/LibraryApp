namespace LibraryApp.Infrastructure;

using LibraryApp.Application.Interfaces;
using LibraryApp.Infrastructure.Persistence;
using LibraryApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration,
    bool useInMemory = false)
    {
        if (useInMemory)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        }

        services.AddScoped<IBookService, BookService>();

        return services;
    }
}
