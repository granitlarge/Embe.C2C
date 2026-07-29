namespace Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;

public record GenerateSearchProfileDescriptionEmbedding(Guid SearchProfileId, string Description) : IWorkItem
{
    public WorkItemType Type => WorkItemType.GenerateSearchProfileDescriptionEmbedding;
}