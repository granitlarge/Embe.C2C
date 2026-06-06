using System.Data;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
namespace Embe.C2C.Application.Commands.Users.Handlers;

public class RegisterHandler
{
    private readonly IC2CContext _context;
    private readonly IFileService _fileService;
    private readonly DomainEventHandler _domainEventHandler;
    private readonly IWorkItemService _workItemService;

    public RegisterHandler
    (
        IC2CContext context,
        IFileService fileService,
        DomainEventHandler domainEventHandler,
        IWorkItemService workItemService
    )
    {
        _context = context;
        _fileService = fileService;
        _domainEventHandler = domainEventHandler;
        _workItemService = workItemService;
    }

    public async Task<TypedResult<RegisterUserFailureReason, User>> HandleAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        var success = false;
        var uploadedFileUrls = new List<string>();

        try
        {
            using var transaction = await _context.BeginTransactionAsync(cancellationToken);
            var registerUserResult = await _context.RegisterUserAsync(command.Email, command.Password, cancellationToken);
            if (!registerUserResult.IsSuccess)
            {
                return TypedResult<RegisterUserFailureReason, User>.Failure(registerUserResult.Reason, registerUserResult.Message!);
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
            var location = new Location(command.Location.Latitude, command.Location.Longitude);
            var files = new HashSet<FileDetails>();
            var identityUserId = registerUserResult.Value!.Id;

            foreach (var file in command.Files)
            {
                var url = await _fileService.UploadFileAsync(file.Url.FromDataUrl(), file.MimeType, cancellationToken);
                uploadedFileUrls.Add(url);
                files.Add(new FileDetails(file.MimeType, url));
            }

            success = true;
            var user = User.Register(email, birthDate, gender, datingPreferences, location, [.. files], identityUserId);
            _context.DomainUsers.Add(user);
            foreach (var domainEvent in user.DomainEvents)
            {
                await _domainEventHandler.HandleAsync(_context, domainEvent, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return TypedResult<RegisterUserFailureReason, User>.Success(user);
        }
        catch (DomainException ex)
        {
            return TypedResult<RegisterUserFailureReason, User>.Failure(RegisterUserFailureReason.DomainError, ex.Message);
        }
        catch (Exception)
        {
            return TypedResult<RegisterUserFailureReason, User>.Failure(RegisterUserFailureReason.Unknown, $"An unexpected error occurred.");
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
    Unknown
}