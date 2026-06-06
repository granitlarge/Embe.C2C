namespace Embe.C2C.Application.Abstractions.Services;

public interface IAuthenticatedUserService
{
    public string? Subject { get; }
    public Guid? UserId { get; }
}