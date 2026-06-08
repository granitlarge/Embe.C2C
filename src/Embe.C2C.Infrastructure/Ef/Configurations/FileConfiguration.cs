using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class FileConfiguration : EntityConfiguration<Domain.Entities.File>
{
    public override void Configure(EntityTypeBuilder<Domain.Entities.File> builder)
    {
        builder.HasKey(f => f.Id);
        builder.ComplexProperty(f => f.FileDetails, fd =>
        {
            fd.Property(f => f.Url);
            fd.Property(f => f.MimeType);
            fd.Property(f => f.Order);
        });
        base.Configure(builder);
    }
}