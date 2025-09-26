using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Emr.Application.Common.Interfaces;
using Emr.Infrastructure.Identity;
using Emr.Infrastructure.Persistence;
using Emr.Infrastructure.Persistence.Interceptors;
using Emr.Infrastructure.Services;
using Azure.Storage.Blobs;
using Microsoft.Identity.Web;

namespace Emr.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        var useInMemoryDatabase = configuration.GetValue<bool>("UseInMemoryDatabase");
        
        if (useInMemoryDatabase)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("EmrDb");
                options.EnableSensitiveDataLogging();
            });
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.EnableSensitiveDataLogging();
            });
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();

        // Authentication
        services.AddAuthentication()
            .AddMicrosoftIdentityWebApi(options =>
            {
                configuration.Bind("AzureAdB2C", options);
            },
            options =>
            {
                configuration.Bind("AzureAdB2C", options);
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("PatientOnly", policy => policy.RequireRole("Patient"));
            options.AddPolicy("StaffOnly", policy => policy.RequireRole("Clinician", "Registrar", "HIM", "OrgAdmin"));
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("OrgAdmin"));
        });

        // Services
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IAuditService, AuditService>();
        services.AddTransient<IQrCodeService, QrCodeService>();
        services.AddTransient<IFhirService, FhirService>();

        // Azure Blob Storage
        services.AddSingleton(x => new BlobServiceClient(configuration.GetConnectionString("AzureStorage")));
        services.AddTransient<IBlobStorageService, BlobStorageService>();

        return services;
    }
}