using System.Reflection;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Users.Handlers;

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
    UserDtoMapper userDtoMapper
) : CommandHandler<AddImagesCommand, ErrorOr<ReadDto<UserDto, UserPermission>>>
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
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;

    protected override async Task<CommandResult<ErrorOr<ReadDto<UserDto, UserPermission>>>> InternalHandleAsync
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

                var uploadImageResult = await _imageService.UploadImageAsync
                (
                    data,
                    1,
                    (int)image.CropOffsetX,
                    (int)image.CropOffsetY,
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

                lock (user)
                {
                    user.AddImage
                    (
                        imageDetails.Value
                    );
                }

            }));

            var dto = await _userDtoMapper.ToDtoAsync(user, user, cancellationToken) ?? throw new InvalidOperationException("user can't read his own data wtf???");
            return new
            (
                true,
                dto
            );

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
                await Task.WhenAll(uploadedImageUrls.Select(url => _workItemService.PerformAsync(new DeleteImage(url), cancellationToken)));
                throw new AggregateException([e1, e2]);
            }
            throw;
        }

    }
}