using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class SearchProfileEmbeddingConfiguration : IEntityTypeConfiguration<SearchProfileEmbedding>
{
    public void Configure(EntityTypeBuilder<SearchProfileEmbedding> builder)
    {
        builder.HasKey(e => e.SearchProfileId);
        builder.HasOne<SearchProfile>()
                .WithOne()
                .HasForeignKey(nameof(SearchProfileEmbedding), nameof(SearchProfileEmbedding.SearchProfileId))
                .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(e => e.Embedding)
            .HasColumnType("vector(1536)");

        builder
            .HasIndex(e => e.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops")
            .HasStorageParameter("m", 16)
            .HasStorageParameter("ef_construction", 64);
    }
}