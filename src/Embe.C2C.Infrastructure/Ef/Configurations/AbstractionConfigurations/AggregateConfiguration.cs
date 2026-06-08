using Embe.C2C.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;

public class AggregateConfiguration<T> : IEntityTypeConfiguration<T> where T : Aggregate
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(a => a.RowVersion).IsRowVersion();
    }
}