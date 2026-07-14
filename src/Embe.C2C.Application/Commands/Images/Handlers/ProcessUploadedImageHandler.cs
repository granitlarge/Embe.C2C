using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Images.Handlers;

public class ProcessUploadedImageHandler
(
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    IContentSafetyService contentSafetyService,
    IImageService imageService
) : CommandHandler<ProcessUploadedImageCommand, Result>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly IContentSafetyService _contentSafetyService = contentSafetyService;
    private readonly IImageService _imageService = imageService;

    protected override async Task<CommandResult<Result>> HandleAsync
    (
        ISparseRepository context,
        ProcessUploadedImageCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var safetyScore = await _contentSafetyService.GetSafetyScoreAsync(command.ImageBytes, cancellationToken);
#warning this hackkkkk (accessing hidden properties) should be fixed
        var user = await context.DomainUsersQuery.FirstOrDefaultAsync(du => EF.Property<List<Image>>(du, "_images").Any(i => i.ImageDetails.Name == command.ImageId.ToString()), cancellationToken);
        var isSafe = Math.Abs(safetyScore) < 0.001m;
        if (user is null)
            return new CommandResult<Result>(false, Result.Failure(FailureReason.NotFound, "user does not exist"));

        var targetImage = user.Images.Single(i => i.ImageDetails.Name == command.ImageId.ToString());
        var newStatus = isSafe ? Domain.ValueObjects.ImageStatus.Accepted : Domain.ValueObjects.ImageStatus.Rejected;
        user.ChangeImageStatus(user.Id, targetImage.Id, newStatus);
        await _imageService.MoveImageAsync(targetImage.ImageDetails.Name, Domain.ValueObjects.ImageStatus.Pending, newStatus);
        return new CommandResult<Result>(Commit: true, Result.Success());
    }
}