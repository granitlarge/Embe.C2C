using System.Data;
using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class RegisterHandler
{
    private readonly C2CContext _context;
    private readonly IFileService _fileService;
    private readonly DomainEventHandler _domainEventHandler;
    private readonly IWorkItemService _workItemService;

    internal RegisterHandler
    (
        C2CContext context,
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

    public async Task<Result<User>> HandleAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        var success = false;
        var uploadedFileUrls = new List<string>();

        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var email = Email.Create(command.Email);
            var birthDate = new BirthDate(command.BirthDate);
            var gender = command.Gender;
            var datingPreferences = new DatingPreferences
            (
                [.. command.DatingPreferences.InterestedInGenders],
                new Age(command.DatingPreferences.AgeRangeMin),
                new Age(command.DatingPreferences.AgeRangeMax),
                command.DatingPreferences.MaximumDistance
            );
            var location = command.Location;
            var files = new HashSet<FileDetails>();

            foreach (var file in command.Files)
            {
                var url = await _fileService.UploadFileAsync(file.Content, file.MimeType, cancellationToken);
                uploadedFileUrls.Add(url);
                files.Add(new FileDetails(file.MimeType, url));
            }

            success = true;
            var user = User.Register(email, birthDate, gender, datingPreferences, location, [.. files]);

            _context.Users.Add(user);
            foreach (var domainEvent in user.DomainEvents)
            {
                await _domainEventHandler.HandleAsync(_context, domainEvent, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<User>.Success(user);
        }
        catch (DomainException ex)
        {
            return Result<User>.Failure(FailureReason.DomainError, ex.Message);
        }
        catch (Exception)
        {
            return Result<User>.Failure(FailureReason.Unknown, $"An unexpected error occurred.");
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