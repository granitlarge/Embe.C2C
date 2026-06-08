using System.Data;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.AuthServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
namespace Embe.C2C.Application.Commands.Users.Handlers;

public class RegisterHandler : TransactionalCommandHandler<RegisterCommand, TypedResult<RegisterUserFailureReason, User>>
{
    private readonly IFileService _fileService;
    private readonly IWorkItemService _workItemService;
    private readonly IAuthService _authService;

    public RegisterHandler
    (
        IC2CContext context,
        IFileService fileService,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IWorkItemService workItemService,
        IAuthService authService
    ) : base(context, domainEventHandler, integrationEventHandler)
    {
        _fileService = fileService;
        _workItemService = workItemService;
        _authService = authService;
    }

    protected override async Task<TransactionalCommandResult<TypedResult<RegisterUserFailureReason, User>>> HandleAsync(ISparseC2CContext context, RegisterCommand command, CancellationToken cancellationToken = default)
    {
        var success = false;
        var uploadedFileUrls = new List<string>();

        try
        {
            var registerUserResult = await _authService.RegisterUserAsync(command.Email, command.Password, cancellationToken);
            if (!registerUserResult.IsSuccess)
            {
                return new TransactionalCommandResult<TypedResult<RegisterUserFailureReason, User>>(false, TypedResult<RegisterUserFailureReason, User>.Failure(registerUserResult.Reason, registerUserResult.Message!));
            }

            var email = Email.Create(command.Email);
            var birthDate = new BirthDate(command.BirthDate);
            var gender = command.Gender;
            var datingPreferences = new DatingPreferences
            (
                [.. command.DatingPreferences.InterestedInGenders],
                new Age(command.DatingPreferences.AgeRangeMin),
                new Age(command.DatingPreferences.AgeRangeMax),
                new Distance(command.DatingPreferences.MaximumDistance.Value, command.DatingPreferences.MaximumDistance.Unit)
            );
            var files = new HashSet<FileDetails>();
            var identityUserId = registerUserResult.Value!.Id;

            foreach (var file in command.Files)
            {
                var url = await _fileService.UploadFileAsync(file.Url.FromDataUrl(), file.MimeType, cancellationToken);
                uploadedFileUrls.Add(url);
                files.Add(new FileDetails(file.MimeType, url, file.Order));
            }

            success = true;

            var user = User.Register(email, birthDate, gender, datingPreferences, location: null, [.. files], identityUserId);
            context.DomainUsers.Add(user);

            return new TransactionalCommandResult<TypedResult<RegisterUserFailureReason, User>>(true, TypedResult<RegisterUserFailureReason, User>.Success(user));
        }
        catch (DomainException ex)
        {
            return new TransactionalCommandResult<TypedResult<RegisterUserFailureReason, User>>(false, TypedResult<RegisterUserFailureReason, User>.Failure(RegisterUserFailureReason.DomainError, ex.Message));
        }
        finally
        {
            if (!success)
            {
                await RemoveUploadedFilesAsync();
            }
        }

        async Task RemoveUploadedFilesAsync()
        {
            try
            {
                await Task.WhenAll(uploadedFileUrls.Select(url => _fileService.DeleteFileAsync(url)));
            }
            catch (Exception)
            {
                await Task.WhenAll(uploadedFileUrls.Select(url => _workItemService.PerformAsync(new DeleteFile(url))));
            }
        }

    }
}

public enum RegisterUserFailureReason
{
    EmailAlreadyExists,
    DomainError,
    WeakPassword,
    Unknown,
    UnknownError
}