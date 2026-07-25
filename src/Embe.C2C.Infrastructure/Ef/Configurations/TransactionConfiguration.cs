using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Transactions;
using Embe.C2C.Domain.Errors.Aggregates;
using Embe.C2C.Infrastructure.Ef.Configurations.AbstractionConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class TransactionConfiguration : AggregateConfiguration<Transaction>
{
    public override void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.ComplexProperty(t => t.Amount, a =>
        {
            a.Property(m => m.Amount);
            a.ComplexProperty(m => m.Currency, c =>
            {
                c.Property(cur => cur.Code);
                c.Property(cur => cur.Symbol);
                c.Property(cur => cur.Name);
            });
        });

        builder.HasOne<Domain.Aggregates.Accounts.Account>()
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        base.Configure(builder);
    }
}