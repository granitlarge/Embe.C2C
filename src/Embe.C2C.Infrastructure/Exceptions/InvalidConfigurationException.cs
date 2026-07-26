namespace Embe.C2C.Infrastructure.Exceptions;

public class MissingConfigurationKeyException(string keyName) : Exception($"Missing required configuration key: '{keyName}'")
{
}