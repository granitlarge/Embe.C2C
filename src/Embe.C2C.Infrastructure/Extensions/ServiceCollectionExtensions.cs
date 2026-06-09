using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Infrastructure.AspNetCore;
using Embe.C2C.Infrastructure.Azure;
using Embe.C2C.Infrastructure.Ef.Contexts;
using Embe.C2C.Infrastructure.Identity;
using Embe.C2C.Infrastructure.SignalR;
using Microsoft.AspNetCore.Identity;
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
        services.AddIdentity<MyIdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.MaxValue;
            options.Lockout.MaxFailedAccessAttempts = 5;
        }).AddEntityFrameworkStores<C2CContext>()
        .AddDefaultTokenProviders();

        services.AddDbContext<IRepository, C2CContext>
        (
            options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite())
        );

        services.AddScoped<IFileService, BlobStorageFileService>();
        services.AddScoped<INotificationService, SignalRNotificationService>();
        services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
        services.AddScoped<IWorkItemService, ServiceBusWorkItemService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}