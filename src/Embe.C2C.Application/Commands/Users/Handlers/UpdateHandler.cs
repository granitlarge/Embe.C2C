using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

using HandlerReturnType = Embe.C2C.Application.Abstractions.EntityWithPermissions<Embe.C2C.Domain.Aggregates.Users.User, System.Collections.Immutable.ImmutableHashSet<Embe.C2C.Application.Authorizations.UserPermission>>;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class UpdateHandler : TransactionalCommandHandler<UpdateCommand, Result<HandlerReturnType>>
{
    private readonly UserAuthorizationPolicy _authorizationPolicy;
    private readonly IFileService _fileService;
    private readonly IWorkItemService _workItemService;

    public UpdateHandler
    (
        IRepository context,
        UserAuthorizationPolicy authorizationPolicy,
        IFileService fileService,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IWorkItemService workItemService
    ) : base(context, domainEventHandler, integrationEventHandler)
    {
        _authorizationPolicy = authorizationPolicy;
        _fileService = fileService;
        _workItemService = workItemService;
    }

    protected override async Task<TransactionalCommandResult<Result<HandlerReturnType>>> HandleAsync(ISparseRepository context, UpdateCommand command, CancellationToken cancellationToken = default)
    {
        var permissions = await _authorizationPolicy.GetPermissionsAsync(command.UserId, cancellationToken);
        if (!permissions.Contains(UserPermission.Update))
        {
            return new TransactionalCommandResult<Result<HandlerReturnType>>(false, Result<HandlerReturnType>.Failure(FailureReason.Forbidden, "User is not authorized to update this profile."));
        }

        var actorId = _authorizationPolicy.GetActorId();

        var success = false;
        HashSet<string> uploadedFileUrls = [];

        try
        {
            var email = Email.Create(command.Email);
            var userName = UserName.Create(command.UserName);
            var birthDate = new BirthDate(command.BirthDate);
            var gender = command.Gender;
            var datingPreferences = new DatingPreferences
            (
                [.. command.DatingPreferences.InterestedInGenders],
                new Age(command.DatingPreferences.AgeRangeMin),
                new Age(command.DatingPreferences.AgeRangeMax),
                new Distance(command.DatingPreferences.MaximumDistance.Value, command.DatingPreferences.MaximumDistance.Unit)
            );
            var location = command.Location != null ? new Location(command.Location.Latitude, command.Location.Longitude) : null;

            var user = await context.DomainUsersQuery.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
            if (user == null)
            {
                return new TransactionalCommandResult<Result<HandlerReturnType>>(false, Result<HandlerReturnType>.Failure(FailureReason.NotFound, "User not found."));
            }


            user.UpdateEmail(actorId, email);
            user.UpdateUserName(actorId, userName);
            user.UpdateBirthDate(actorId, birthDate);
            user.UpdateGender(actorId, gender);
            user.UpdatePreferences(actorId, datingPreferences);
            user.UpdateLocation(actorId, location);

            var filesToRemove = user.Files.Where(f => !command.FilesToKeep.Contains(f.Id)).ToList();
            foreach (var file in filesToRemove)
            {
                user.RemoveFile(actorId, file.Id);
                await _fileService.DeleteFileByUrlAsync(file.FileDetails.Name, cancellationToken);
            }

            foreach (var file in command.FilesToAdd)
            {
                var uploadFileResult = await _fileService.UploadFileAsync(file.Url.FromDataUrl(), file.MimeType, cancellationToken);
                user.AddFile(actorId, new FileDetails(uploadFileResult.Name, file.MimeType, file.Order));
                uploadedFileUrls.Add(uploadFileResult.Url);
            }

            success = true;
            return new TransactionalCommandResult<Result<HandlerReturnType>>(true, Result<HandlerReturnType>.Success(new HandlerReturnType(user, permissions)));
        }
        catch (DomainException)
        {
            return new TransactionalCommandResult<Result<HandlerReturnType>>(false, Result<HandlerReturnType>.Failure(FailureReason.DomainError, "Invalid input data."));
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