using Backend.Application.Abstractions;
using Backend.Infrastructure.Persistence;
using Backend.Infrastructure.Repositories;
using Backend.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // PostgreSQL via EF Core
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomerDocumentRepository, CustomerDocumentRepository>();
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<IInsurancePackageRepository, InsurancePackageRepository>();
        services.AddScoped<IPolicyTermRepository, PolicyTermRepository>();
        services.AddScoped<IInsurancePolicyRepository, InsurancePolicyRepository>();
        services.AddScoped<IClaimRepository, ClaimRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddSingleton<IPasswordHasher, PasswordHasherService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Email Configuration
        services.Configure<Backend.Infrastructure.Services.Email.EmailSettings>(
            configuration.GetSection("EmailSettings"));
        services.AddTransient<IEmailService, Backend.Infrastructure.Services.Email.SmtpEmailService>();

        // MinIO Storage
        services.Configure<Backend.Infrastructure.Services.Storage.MinioSettings>(
            configuration.GetSection("MinioSettings"));
        services.AddSingleton<IFileStorageService, Backend.Infrastructure.Services.Storage.MinioStorageService>();

        return services;
    }
}
