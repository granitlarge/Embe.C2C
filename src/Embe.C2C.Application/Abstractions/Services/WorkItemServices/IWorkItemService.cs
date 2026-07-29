using System.Text.Json.Serialization;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;

namespace Embe.C2C.Application.Abstractions.Services.WorkItemServices;

public enum WorkItemType
{
    GenerateSearchProfileDescriptionEmbedding,
    DeleteImage
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = nameof(Type))]
[JsonDerivedType(typeof(DeleteImage), (int)WorkItemType.DeleteImage)]
[JsonDerivedType(typeof(GenerateSearchProfileDescriptionEmbedding), (int)WorkItemType.GenerateSearchProfileDescriptionEmbedding)]
public interface IWorkItem
{
    public WorkItemType Type { get; }
}

public interface IWorkItemService
{
    public Task PerformAsync<T>(T task, CancellationToken cancellationToken = default)
    where T : IWorkItem;
}