using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public record AddImageResponse
(
    string? UploadUrl,
    Image Image
);

public class AddImageHandler : CommandHandler<AddImageCommand, Result<AddImageResponse>>
{
    private readonly IImageService _fileService;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public AddImageHandler
    (
        IImageService fileService,
        DomainEventStore domainEventStore,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthenticatedUserService authenticatedUserService
    ) : base
    (
        domainEventStore,
        context,
        domainEventHandler,
        integrationEventHandler
    )
    {
        _fileService = fileService;
        _authenticatedUserService = authenticatedUserService;
    }

    protected async override Task<CommandResult<Result<AddImageResponse>>> HandleAsync
    (
        ISparseRepository context,
        AddImageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("user is not authenticated");
        var user = await context.DomainUsersQuery.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return new CommandResult<Result<AddImageResponse>>
            (
                Commit: false,
                Result<AddImageResponse>.Failure(FailureReason.NotFound, "user does not exist")
            );
        }

        var fileName = Guid.CreateVersion7().ToString();
        var image = user.AddImage(userId, new Domain.ValueObjects.ImageDetails(fileName, command.MimeType, command.Order, Domain.ValueObjects.ImageStatus.Pending));
        var sas = await _fileService.GenerateImageSasUrlAsync(fileName, Domain.ValueObjects.ImageStatus.Pending, FilePermissions.Write, TimeSpan.FromDays(1), cancellationToken);
        return new CommandResult<Result<AddImageResponse>>(true, Result<AddImageResponse>.Success(new AddImageResponse(sas, image)));
    }
}