namespace Embe.C2C.Application.Abstractions.Services;

public interface IEmailService
{
    public Task SendAsync
    (
        string email, 
        string subject, 
        string htmlContent, 
        string plainText, 
        CancellationToken cancellationToken
    );
}