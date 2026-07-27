namespace Embe.C2C.Application.Abstractions;

public interface ILoggerFactory
{
    ILogger<T> Create<T>();
}

public interface ILogger<T>
{
    Task TraceAsync(string message);
    Task DebugAsync(string message);
    Task InformationAsync(string message);
    Task WarningAsync(string message);
    Task ErrorAsync(string message);
    Task CriticalAsync(string message);
}