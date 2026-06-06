using Embe.C2C.Domain.Aggregates.Accounts;
using Embe.C2C.Domain.Aggregates.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Embe.C2C.Infrastructure.Ef.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
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

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}