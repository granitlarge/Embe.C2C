namespace Embe.C2C.Application.Commands.SearchProfiles;

public record CreateSearchProfileEmbeddingCommand(Guid SearchProfileId, string Content);