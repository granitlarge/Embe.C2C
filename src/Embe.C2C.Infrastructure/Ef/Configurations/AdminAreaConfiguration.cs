using Embe.C2C.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class AdminAreaConfiguration : IEntityTypeConfiguration<AdminArea>
{
    public void Configure(EntityTypeBuilder<AdminArea> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(50);
        builder.Property(x => x.ParentId).HasMaxLength(50);
        builder
            .HasOne<AdminArea>()
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasIndex(x => x.ParentId);
    }
}