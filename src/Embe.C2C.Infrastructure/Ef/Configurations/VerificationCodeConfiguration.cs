using Embe.C2C.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<VerificationCode> builder)
    {
        builder.HasKey(vc => vc.Id);
    }
}