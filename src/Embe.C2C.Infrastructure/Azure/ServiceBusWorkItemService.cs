using Azure.Messaging.ServiceBus;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Microsoft.Extensions.Configuration;

namespace Embe.C2C.Infrastructure.Azure;

public class ServiceBusWorkItemService : IWorkItemService
{
    private readonly ServiceBusClient _serviceBusClient;

    public ServiceBusWorkItemService(IConfiguration configuration)
    {
        _serviceBusClient = new ServiceBusClient(configuration.GetConnectionString("AzureServiceBus"));
    }

    public async Task PerformAsync<T>(T task, CancellationToken cancellationToken = default)
        where T : IWorkItem
    {
        await _serviceBusClient
            .CreateSender("work-items")
            .SendMessageAsync(new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(task)), cancellationToken);
    }
}