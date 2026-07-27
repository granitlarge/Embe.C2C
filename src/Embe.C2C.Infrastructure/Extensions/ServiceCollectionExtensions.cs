using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Settings;
using Embe.C2C.Application.Services;
using Embe.C2C.Infrastructure.AspNetCore;
using Embe.C2C.Infrastructure.Azure;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Embe.C2C.Infrastructure.Ef.Repositories;
using Embe.C2C.Infrastructure.Identity;
using Embe.C2C.Infrastructure.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.SignalR.Management;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Embe.C2C.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure
    (
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment
    )
    {
        services.AddScoped<Settings>();
        services.AddScoped<ISettings, Settings>();
        services.AddIdentityCore<MyIdentityUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(3650);
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
            .AddEntityFrameworkStores<C2CContext>()
            .AddDefaultTokenProviders();

        services.AddDbContext<IRepository, C2CContext>
        (
            options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite())
        );

        services.AddScoped<IImageService, BlobStorageImageService>();
        services.AddScoped<IRealTimeNotificationService, SignalRNotificationService>();
        services.AddScoped<IRealTimeUpdateService, SignalRNotificationService>();
        services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
        services.AddScoped<IWorkItemService, ServiceBusWorkItemService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IMatchingRepository, MatchingRepository>();
        services.AddScoped<IBlockingRepository, BlockingRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<ISearchProfileRepository, SearchProfileRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAdminAreaRepository, AdminAreaRepository>();
        services.AddScoped<ICandidateRepository, CandidateRepository>();

        if (environment.IsDevelopment())
        {
            services.AddScoped<IContentSafetyService, NullContentSafetyService>();
            services.AddScoped<IEmailService, NullEmailService>();
        }
        else
        {
            services.AddScoped<IContentSafetyService, AzureAIContentSafetyService>();
            services.AddScoped<IEmailService, AzureCommunicationServicesEmailService>();
        }

        services.AddSingleton((serviceProvider) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var serviceManager = new ServiceManagerBuilder()
                .WithOptions(option =>
            {
                option.ConnectionString = configuration.GetConnectionString("AzureSignalR") ?? configuration.GetValue<string>("AzureSignalR");
            }).BuildServiceManager();
            var pool = new SignalRServiceHubContextPool(serviceManager);
            return pool;
        });

        return services;
    }
}