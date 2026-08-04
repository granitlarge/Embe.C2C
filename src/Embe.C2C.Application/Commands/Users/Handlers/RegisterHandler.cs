using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Errors;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.ValueObjects;
using ErrorOr;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class RegisterHandler : CommandHandler<RegisterCommand, ErrorOr<Credentials>>
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthService _authService;
    private readonly IImageService _imageService;
    private readonly IWorkItemService _workItemService;

    public RegisterHandler
    (
        IUserRepository userRepo,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IAuthService authService,
        DomainEventStore domainEventStore,
        IImageService imageService,
        IWorkItemService workItemService
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _authService = authService;
        _userRepo = userRepo;
        _imageService = imageService;
        _workItemService = workItemService;
    }


    private async Task<ErrorOr<ImageDetails[]>> UploadImagesAsync(ImageWriteDto[] dtos, CancellationToken cancellationToken)
    {
        var uploadedImageUrls = new List<string>();
        var tasks = dtos.Select(async dto =>
        {
            var result = await _imageService.UploadImageAsync
            (
                Convert.FromBase64String(dto.Base64EncodedImageData),
                (int)dto.CropOffsetX,
                (int)dto.CropOffsetY,
                (int)dto.Width,
                (int)dto.Height,
                cancellationToken
            );

            lock (uploadedImageUrls)
            {
                uploadedImageUrls.Add(result.OriginalUrl);
                uploadedImageUrls.Add(result.LargeUrl);
                uploadedImageUrls.Add(result.MediumUrl);
                uploadedImageUrls.Add(result.SmallUrl);
            }

            return (dto, result);
        });

        try
        {
            var uploadImageResults = await Task.WhenAll(tasks);
            var imageDetails = uploadImageResults.Select(i => ImageDetails.Create(i.result.Name, i.dto.MimeType, i.dto.Order));
            foreach (var imageDetail in imageDetails)
            {
                if (imageDetail.IsError)
                    return imageDetail.Errors;
            }
            return imageDetails.Select(id => id.Value).ToArray();
        }
        catch (Exception)
        {
            try
            {
                await Task.WhenAll(uploadedImageUrls.Select(i => _imageService.DeleteImageByUrlAsync(i, CancellationToken.None)));
            }
            catch (Exception)
            {
                await Task.WhenAll(uploadedImageUrls.Select(ui => _workItemService.PerformAsync(WorkItem.Create(new DeleteImage(ui), WorkItemType.DeleteImage))));
                throw;
            }
            throw;
        }
    }

    protected override async Task<CommandResult<ErrorOr<Credentials>>> InternalHandleAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        var errors = new List<Error>();
        var email = Email.Create(command.Email).ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.Email))));
        var alias = Alias.Create(command.Alias).ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.Alias))));
        var birthDate = BirthDate.Create(command.BirthDate).ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.BirthDate))));
        var location = command.Location != null ? Location.Create(command.Location.Latitude, command.Location.Longitude).ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.Location)))) : default;
        var images = (await UploadImagesAsync(command.Images, cancellationToken)).ElseDo(e => errors.AddRange(e.WithPropertyName(nameof(command.Images))));

        if (errors.Count != 0)
        {
            return new
            (
                false,
                ErrorOrFactory.From<Credentials>(errors)
            );
        }

        var files = new HashSet<ImageDetails>();
        var user = User
            .Register
            (
                email.Value,
                alias.Value,
                birthDate.Value,
                gender: command.Gender,
                location: location != default ? location.Value : null,
                images: [.. images.Value],
                bio: null
            )
            .ElseDo(errors.AddRange);

        if (errors.Count != 0)
        {
            return new
            (
                false,
                ErrorOrFactory.From<Credentials>(errors)
            );
        }

        var isValidVerificationCode = await _authService.VerifyVerificationCodeAsync(command.Email, command.EmailVerificationCode, cancellationToken);
        if (!isValidVerificationCode)
        {
            return new
            (
                false,
                ApplicationErrors.InvalidVerificationCode.ToValidationErrorOr()
            );
        }

        _userRepo.Set.Add(user.Value);
        await _userRepo.SaveChangesAsync(cancellationToken);

        var registerUserResult = await _authService.RegisterUserAsync(user.Value.Id, command.Email, command.Password, cancellationToken);
        if (!registerUserResult.IsSuccess)
        {
            return new
            (
                false,
                ErrorOrFactory.From<Credentials>(registerUserResult.Errors)
            );
        }

        var signInResult = await _authService.SignInAsync(email.Value.Value, command.Password, cancellationToken);
        if (!signInResult.IsSuccess)
        {
            throw new NotImplementedException();
        }

        return new
        (
            true,
            ErrorOrFactory.From(signInResult.Value!)
        );
    }
}