using Pgvector;

namespace Embe.C2C.Infrastructure.Ef.Entities;

public class SearchProfileEmbedding
{
    public Guid SearchProfileId { get; set; }
    public Vector Embedding { get; set; } = null!;
}