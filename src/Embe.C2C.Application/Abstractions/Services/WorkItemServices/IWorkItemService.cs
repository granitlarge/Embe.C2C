namespace Embe.C2C.Application.Abstractions.Services.WorkItemServices;

public interface IWorkItem
{

}

public interface IWorkItemService
{
    public Task PerformAsync<T>(T task, CancellationToken cancellationToken = default)
    where T : IWorkItem;
}