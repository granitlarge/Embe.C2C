using Microsoft.Azure.SignalR.Management;

namespace Embe.C2C.Infrastructure.SignalR;

public class SignalRServiceHubContextPool
{
    private readonly ServiceManager _serviceManager;
    private ServiceHubContext? _serviceHubContext;

    public SignalRServiceHubContextPool(ServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    public async Task<ServiceHubContext> GetHubContextAsync(CancellationToken cancellationToken)
    {
        if (_serviceHubContext is null)
        {
            _serviceHubContext = await _serviceManager.CreateHubContextAsync("mainHub", cancellationToken); ;
        }
        return _serviceHubContext;
    }
}