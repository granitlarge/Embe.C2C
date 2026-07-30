using Azure.Messaging.ServiceBus;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Microsoft.Extensions.Configuration;

namespace Embe.C2C.Infrastructure.Azure;

public class ServiceBusWorkItemService : IWorkItemService
{
    private readonly ServiceBusClient _serviceBusClient;

    public ServiceBusWorkItemService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureServiceBus") ?? configuration.GetValue<string>("AzureServiceBus");
        _serviceBusClient = new ServiceBusClient(connectionString);
    }

    public async Task PerformAsync(WorkItem task, CancellationToken cancellationToken = default)
    {
        const string queueName = "work-items";
        await _serviceBusClient
            .CreateSender(queueName)
            .SendMessageAsync(new ServiceBusMessage(System.Text.Json.JsonSerializer.Serialize(task)), cancellationToken);
    }
}