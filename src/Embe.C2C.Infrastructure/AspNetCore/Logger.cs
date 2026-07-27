using Embe.C2C.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Embe.C2C.Infrastructure.AspNetCore;

public class LoggerFactory : Application.Abstractions.ILoggerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public LoggerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Application.Abstractions.ILogger<T> Create<T>()
    {
        var logger = _serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<T>>();
        return new Logger<T>(logger);
    }
}

public class Logger<T>(Microsoft.Extensions.Logging.ILogger<T> logger) : Application.Abstractions.ILogger<T>
{
    private readonly Microsoft.Extensions.Logging.ILogger<T> _logger = logger;

    public Task CriticalAsync(string message)
    {
        _logger.LogCritical(message);
        return Task.CompletedTask;
    }

    public Task DebugAsync(string message)
    {
        _logger.LogDebug(message);
        return Task.CompletedTask;
    }

    public Task ErrorAsync(string message)
    {
        _logger.LogError(message);
        return Task.CompletedTask;
    }

    public Task InformationAsync(string message)
    {
        _logger.LogInformation(message);
        return Task.CompletedTask;
    }

    public Task TraceAsync(string message)
    {
        _logger.LogTrace(message);
        return Task.CompletedTask;
    }

    public Task WarningAsync(string message)
    {
        _logger.LogWarning(message);
        return Task.CompletedTask;
    }
}