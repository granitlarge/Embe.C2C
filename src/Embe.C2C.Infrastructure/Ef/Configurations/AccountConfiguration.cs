using Embe.C2C.Domain.Aggregates.Transactions;
using Embe.C2C.Domain.Errors.Aggregates;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class AccountConfiguration : AggregateConfiguration<Domain.Aggregates.Accounts.Account>
{
    public override void Configure(EntityTypeBuilder<Domain.Aggregates.Accounts.Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder
            .ComplexProperty(a => a.Balance, b =>
            {
                b.Property(m => m.Amount);
                b.ComplexProperty(m => m.Currency, c =>
                {
                    c.Property(c => c.Code);
                    c.Property(c => c.Symbol);
                    c.Property(c => c.Name);
                });
            });

        builder
            .HasMany<Transaction>()
            .WithOne()
            .HasForeignKey(t => t.AccountId);

        builder.HasOne<Domain.Aggregates.Users.User>()
            .WithOne()
            .HasForeignKey<Domain.Aggregates.Accounts.Account>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        base.Configure(builder);
    }
}