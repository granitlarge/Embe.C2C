namespace Embe.C2C.Application.Abstractions.Services.WorkItemServices;

public enum WorkItemType
{
    GenerateSearchProfileDescriptionEmbedding,
    DeleteImage
}
public interface IWorkItem
{
    public WorkItemType Type { get; }
}

public interface IWorkItemService
{
    public Task PerformAsync<T>(T task, CancellationToken cancellationToken = default)
    where T : IWorkItem;
}