using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Entities;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public record AddImagesResult
(
    ImageDto[] Images
);

public class AddImagesHandler
(
    IContentSafetyService contentSafetyService,
    IImageService imageService,
    IAuthenticatedUserService authenticatedUserService,
    IUserRepository userRepository,
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    IWorkItemService workItemService,
    ILoggerFactory loggerFactory,
    ImageDtoMapper imageDtoMapper
) : CommandHandler<AddImagesCommand, ErrorOr<AddImagesResult>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IImageService _imageService = imageService;
    private readonly IContentSafetyService _contentSafetyService = contentSafetyService;
    private readonly IWorkItemService _workItemService = workItemService;
    private readonly ImageDtoMapper _imageDtoMapper = imageDtoMapper;
    private readonly ILogger<AddImagesHandler> _logger = loggerFactory.Create<AddImagesHandler>();

    protected override async Task<CommandResult<ErrorOr<AddImagesResult>>> InternalHandleAsync
    (
        AddImagesCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("user is not authenticated");
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return new
            (
                false,
                ApplicationErrors.NotFound.ToNotFoundErrorOr()
            );
        }

        var uploadedImageUrls = new List<string>();
        var addedImages = new List<Image>();

        try
        {
            await Task.WhenAll(command.Images.Select(async image =>
            {
                var data = Convert.FromBase64String(image.Base64EncodedImageData);
                var safetyScore = await _contentSafetyService.GetSafetyScoreAsync(data, cancellationToken);
                if (safetyScore > .001m)
                {
                    return;
                }

                await _logger.TraceAsync($"Uploading image: {image.Width}x{image.Height} - (X,Y) = {image.CropOffsetX},{image.CropOffsetY}");
                var uploadImageResult = await _imageService.UploadImageAsync
                (
                    data,
                    (int)image.CropOffsetX,
                    (int)image.CropOffsetY,
                    (int)image.Width,
                    (int)image.Height,
                    cancellationToken
                );

                lock (uploadedImageUrls)
                {
                    uploadedImageUrls.Add(uploadImageResult.OriginalUrl);
                    uploadedImageUrls.Add(uploadImageResult.LargeUrl);
                    uploadedImageUrls.Add(uploadImageResult.MediumUrl);
                    uploadedImageUrls.Add(uploadImageResult.SmallUrl);
                }

                var imageDetails = Domain.ValueObjects.ImageDetails.Create
                (
                    uploadImageResult.Name,
                    image.MimeType,
                    image.Order
                );

                if (imageDetails.IsError)
                {
                    throw new InvalidOperationException("Failed to create image");
                }

                ErrorOr<Image> addImageResult;
                lock (user)
                {
                    addImageResult = user.AddImage
                    (
                        imageDetails.Value
                    );
                }

                if (addImageResult.IsError)
                {
                    throw new InvalidOperationException("Failed to add image.");
                }
                else
                {

                    lock (addedImages)
                    {
                        addedImages.Add(addImageResult.Value);
                    }

                }

            }));

            await _userRepository.SaveChangesAsync(cancellationToken);

            var result = await Task.WhenAll(addedImages.Select(image => _imageDtoMapper.ToDtoAsync(image, cancellationToken)));
            return new(true, new AddImagesResult(result));

        }
        catch (Exception e1)
        {
            var tasks = uploadedImageUrls.Select(url => _imageService.DeleteImageByUrlAsync(url, cancellationToken));
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception e2)
            {
                await Task.WhenAll(uploadedImageUrls.Select(url => _workItemService.PerformAsync(WorkItem.Create(new DeleteImage(url), WorkItemType.DeleteImage), cancellationToken)));
                throw new AggregateException([e1, e2]);
            }
            throw;
        }

    }
}