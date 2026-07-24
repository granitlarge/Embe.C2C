using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Images.Handlers;

public class ProcessUploadedImageHandler
(
    IUserRepository userRepo,
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    IContentSafetyService contentSafetyService
) : CommandHandler<ProcessUploadedImageCommand, Result>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IContentSafetyService _contentSafetyService = contentSafetyService;

    protected override async Task<CommandResult<Result>> InternalHandleAsync
    (
        ProcessUploadedImageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var safetyScore = await _contentSafetyService.GetSafetyScoreAsync(command.ImageBytes, cancellationToken);
        var user = await _userRepo.GetImageOwnerAsync(command.ImageName, cancellationToken);
        if (user is null)
        {
            Console.WriteLine("ERORR: Failed to find user that owns image.");
            return new CommandResult<Result>(true, Result.Failure(FailureReason.NotFound, "user not found"));
        }

        var isSafe = Math.Abs(safetyScore) < 0.001m;
        var targetImage = user.Images.Single(i => i.ImageDetails.Name == command.ImageName);
        var newStatus = isSafe ? ImageStatus.Accepted : ImageStatus.Rejected;

        user.ChangeImageStatus(user.Id, targetImage.Id, newStatus);
        if (newStatus == ImageStatus.Rejected)
        {
            user.RemoveImage(user.Id, targetImage.Id);
        }
        return new CommandResult<Result>(Save: true, Result.Success());
    }
}