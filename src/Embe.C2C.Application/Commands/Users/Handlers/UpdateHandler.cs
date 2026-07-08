using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class UpdateHandler : TransactionalCommandHandler<UpdateCommand, Result<ReadDto<UserDto, UserPermission>?>>
{
    private readonly IAuthenticatedUserService _user;
    private readonly UserAuthorizationService _authorizationPolicy;
    private readonly IFileService _fileService;
    private readonly IWorkItemService _workItemService;
    private readonly UserDtoMapper _userDtoMapper;

    public UpdateHandler
    (
        IAuthenticatedUserService user,
        IRepository context,
        UserAuthorizationService authorizationPolicy,
        IFileService fileService,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IWorkItemService workItemService,
        DomainEventStore domainEventStore,
        UserDtoMapper userDtoMapper
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _user = user;
        _authorizationPolicy = authorizationPolicy;
        _fileService = fileService;
        _workItemService = workItemService;
        _userDtoMapper = userDtoMapper;
    }

    protected override async Task<TransactionalCommandResult<Result<ReadDto<UserDto, UserPermission>?>>> HandleAsync
    (
        ISparseRepository context,
        UpdateCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var (permissions, variant) = await _authorizationPolicy.GetAsync(command.UserId, cancellationToken);
        if (!permissions.Contains(UserPermission.Update))
        {
            return new TransactionalCommandResult<Result<ReadDto<UserDto, UserPermission>?>>(false, Result<ReadDto<UserDto, UserPermission>?>.Failure(FailureReason.Forbidden, "User is not authorized to update this profile."));
        }

        var actorId = _user.UserId ?? throw new InvalidOperationException("User is not authenticated.");

        var success = false;
        HashSet<string> uploadedFileUrls = [];

        try
        {
            var alias = Alias.Create(command.Alias);
            var birthDate = new BirthDate(command.BirthDate);
            var gender = command.Gender;
            var location = command.Location != null ? new Location(command.Location.Latitude, command.Location.Longitude) : null;

            var user = await context.DomainUsersQuery.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
            if (user == null)
            {
                return new TransactionalCommandResult<Result<ReadDto<UserDto, UserPermission>?>>(false, Result<ReadDto<UserDto, UserPermission>?>.Failure(FailureReason.NotFound, "User not found."));
            }

            user.UpdateAlias(actorId, alias);
            user.UpdateBirthDate(actorId, birthDate);
            user.UpdateGender(actorId, gender);
            user.UpdateLocation(actorId, location);

            var imagesToRemove = user.Images.Where(f => !command.ImagesToKeep?.Any(itk => itk.Id == f.Id) ?? true).ToList();
            foreach (var image in imagesToRemove)
            {
                user.RemoveImage(actorId, image.Id);
                await _fileService.DeleteFileByNameAsync(image.ImageDetails.Name, cancellationToken);
            }

            foreach (var image in command.ImagesToKeep ?? [])
            {
                user.ChangeImageOrder(actorId, image.Id, image.Order);
            }

            foreach (var image in command.ImagesToAdd ?? [])
            {
                var uploadImageResult = await _fileService.UploadFileAsync(image.Url.DataUrlToBytes(), image.MimeType, cancellationToken);
                user.AddImage(actorId, new ImageDetails(uploadImageResult.Name, image.MimeType, image.Order));
                uploadedFileUrls.Add(uploadImageResult.Url);
            }

            success = true;

            var dto = await _userDtoMapper.ToDtoAsync(user, variant, cancellationToken);
            var readDto = new ReadDto<UserDto, UserPermission>(dto!, permissions);

            return new TransactionalCommandResult<Result<ReadDto<UserDto, UserPermission>?>>(true, Result<ReadDto<UserDto, UserPermission>?>.Success(readDto));
        }
        catch (DomainException)
        {
            return new TransactionalCommandResult<Result<ReadDto<UserDto, UserPermission>?>>(false, Result<ReadDto<UserDto, UserPermission>?>.Failure(FailureReason.DomainError, "Invalid input data."));
        }
        finally
        {
            if (!success)
            {
                try
                {
                    await Task.WhenAll(uploadedFileUrls.Select(url => _fileService.DeleteFileByUrlAsync(url, cancellationToken)));
                }
                catch (Exception)
                {
                    await Task.WhenAll(uploadedFileUrls.Select(url => _workItemService.PerformAsync(new DeleteFile(url))));
                }
            }
        }

    }
}