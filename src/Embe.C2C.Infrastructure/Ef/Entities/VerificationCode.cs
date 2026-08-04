namespace Embe.C2C.Infrastructure.Ef.Entities;

public class VerificationCode
{
    public string Id { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int RedemptionAttempts { get; set; }
}