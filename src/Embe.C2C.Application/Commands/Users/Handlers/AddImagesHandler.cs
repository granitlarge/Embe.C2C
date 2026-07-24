using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;

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
    UserAuthorizationService userAuthorizationService,
    UserDtoMapper userDtoMapper
) : CommandHandler<AddImagesCommand, Result<ReadDto<UserDto, UserPermission>>>
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
    private readonly UserAuthorizationService _userAuthorizationService = userAuthorizationService;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;

    protected override async Task<CommandResult<Result<ReadDto<UserDto, UserPermission>>>> InternalHandleAsync
    (
        AddImagesCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("user is not authenticated");
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return new CommandResult<Result<ReadDto<UserDto, UserPermission>>>
            (
                false,
                Result<ReadDto<UserDto, UserPermission>>.Failure
                (
                    FailureReason.NotFound,
                    "user does not exist"
                )
            );
        }

        var uploadedImageUrls = new List<string>();
        try
        {

            foreach (var image in command.Images)
            {
                var data = Convert.FromBase64String(image.Base64EncodedImageData);
                var safetyScore = await _contentSafetyService.GetSafetyScoreAsync(data, cancellationToken);
                if (safetyScore > .001m)
                {
                    continue;
                }

                var uploadImageResult = await _imageService.UploadImageAsync
                (
                    data,
                    1000,
                    (int)(1000 * 2.1),
                    (int)image.CropOffsetX,
                    (int)image.CropOffsetY,
                    cancellationToken
                );

                uploadedImageUrls.Add(uploadImageResult.OriginalUrl);
                uploadedImageUrls.Add(uploadImageResult.LargeUrl);
                uploadedImageUrls.Add(uploadImageResult.MediumUrl);
                uploadedImageUrls.Add(uploadImageResult.SmallUrl);

                user.AddImage
                (
                    userId,
                    new Domain.ValueObjects.ImageDetails
                    (
                       uploadImageResult.Name,
                       image.MimeType,
                       image.Order
                    )
                );
            }

            var dto = await user.Enrich(user).ToDtoAsync(_userAuthorizationService, _userDtoMapper, cancellationToken) ??
                throw new InvalidOperationException("User can't read his own data wtf???");

            return new CommandResult<Result<ReadDto<UserDto, UserPermission>>>
            (
                Save: true,
                Result<ReadDto<UserDto, UserPermission>>.Success(dto)
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