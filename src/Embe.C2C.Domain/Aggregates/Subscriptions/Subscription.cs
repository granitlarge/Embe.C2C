using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Domain.Aggregates.Subscriptions;

public class Subscription : Aggregate
{

    public static readonly Money WeeklyPrice = Money.Create(5, new Currency("USD", "United States Dollar", "$")).Value;
    public static readonly Money MonthlyPrice = Money.Create(10, new Currency("USD", "United States Dollar", "$")).Value;

    private Subscription
    (
        Guid userId
    )
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public DateOnly? PaidThroughDate { get; private set; }
    public DateOnly? NextBillingDate { get; private set; }

    public bool IsActive => PaidThroughDate.HasValue && PaidThroughDate.Value > DateOnly.FromDateTime(DateTime.UtcNow);

}

public enum BillingFrequency
{
    Weekly,
    Monthly
}