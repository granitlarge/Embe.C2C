using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<Domain.Entities.File>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.File> builder)
    {
        builder.HasKey(f => f.Id);
        builder.ComplexProperty(f => f.FileDetails, fd =>
        {
            fd.Property(f => f.Url);
            fd.Property(f => f.MimeType);
        });
    }
}