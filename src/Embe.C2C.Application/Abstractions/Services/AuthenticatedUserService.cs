namespace Embe.C2C.Application.Abstractions.Services;

public interface IAuthenticatedUserService
{
    public Guid? UserId { get; }
}