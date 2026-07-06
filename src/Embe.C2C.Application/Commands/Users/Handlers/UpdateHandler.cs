using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class UpdateHandler : TransactionalCommandHandler<UpdateCommand, Result<ReadDto<UserDto, UserPermission>?>>
{
    private readonly IAuthenticatedUserService _user;
    private readonly UserAuthorizationPolicy _authorizationPolicy;
    private readonly IFileService _fileService;
    private readonly IWorkItemService _workItemService;

    public UpdateHandler
    (
        IAuthenticatedUserService user,
        IRepository context,
        UserAuthorizationPolicy authorizationPolicy,
        IFileService fileService,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IWorkItemService workItemService,
        DomainEventStore domainEventStore
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _user = user;
        _authorizationPolicy = authorizationPolicy;
        _fileService = fileService;
        _workItemService = workItemService;
    }

    protected override async Task<TransactionalCommandResult<Result<ReadDto<UserDto, UserPermission>?>>> HandleAsync(ISparseRepository context, UpdateCommand command, CancellationToken cancellationToken = default)
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
            var email = Email.Create(command.Email);
            var userName = Alias.Create(command.UserName);
            var birthDate = new BirthDate(command.BirthDate);
            var gender = command.Gender;
            var location = command.Location != null ? new Location(command.Location.Latitude, command.Location.Longitude) : null;

            var user = await context.DomainUsersQuery.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
            if (user == null)
            {
                return new TransactionalCommandResult<Result<ReadDto<UserDto, UserPermission>?>>(false, Result<ReadDto<UserDto, UserPermission>?>.Failure(FailureReason.NotFound, "User not found."));
            }

            user.UpdateEmail(actorId, email);
            user.UpdateAlias(actorId, userName);
            user.UpdateBirthDate(actorId, birthDate);
            user.UpdateGender(actorId, gender);
            user.UpdateLocation(actorId, location);

            var filesToRemove = user.Images.Where(f => !command.FilesToKeep.Contains(f.Id)).ToList();
            foreach (var file in filesToRemove)
            {
                user.RemoveImage(actorId, file.Id);
                await _fileService.DeleteFileByUrlAsync(file.ImageDetails.Name, cancellationToken);
            }

            foreach (var file in command.FilesToAdd)
            {
                var uploadFileResult = await _fileService.UploadFileAsync(file.Url.DataUrlToBytes(), file.MimeType, cancellationToken);
                user.AddImage(actorId, new ImageDetails(uploadFileResult.Name, file.MimeType, file.Order));
                uploadedFileUrls.Add(uploadFileResult.Url);
            }

            success = true;

            var fileGenerator = new FileUrlGenerator(_fileService, TimeSpan.FromMinutes(15));
            return new TransactionalCommandResult<Result<ReadDto<UserDto, UserPermission>?>>(true, Result<ReadDto<UserDto, UserPermission>?>.Success(await _authorizationPolicy.ToDtoAsync(user, cancellationToken)));
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