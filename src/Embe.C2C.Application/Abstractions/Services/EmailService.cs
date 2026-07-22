namespace Embe.C2C.Application.Abstractions.Services;

public interface IEmailService
{
    Task SendAsync
    (
        string email,
        string subject,
        string body
    );
}