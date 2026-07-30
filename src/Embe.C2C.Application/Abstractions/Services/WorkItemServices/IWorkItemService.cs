using System.Text.Json;
using System.Text.Json.Serialization;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;

namespace Embe.C2C.Application.Abstractions.Services.WorkItemServices;

public enum WorkItemType
{
    GenerateSearchProfileDescriptionEmbedding,
    DeleteImage
}

public record WorkItem
{
    [JsonConstructor]
    private WorkItem(string payload, WorkItemType type)
    {
        Type = type;
        Payload = payload;
    }

    public string Payload { get; }

    public WorkItemType Type { get; }

    public T? As<T>()
    {
        return JsonSerializer.Deserialize<T>(Payload);
    }

    public static WorkItem Create<T>(T payload, WorkItemType type)
    {
        return new WorkItem(JsonSerializer.Serialize(payload), type);
    }
}

public interface IWorkItemService
{
    public Task PerformAsync<T>(T task, CancellationToken cancellationToken = default)
    where T : WorkItem;
}