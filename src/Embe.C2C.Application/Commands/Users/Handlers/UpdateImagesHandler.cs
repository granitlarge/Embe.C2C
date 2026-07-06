using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Dtos;
using Embe.C2C.Application.Dtos.Read.Entities;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions;
using Embe.C2C.Domain;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class UpdateImagesHandler : TransactionalCommandHandler<UpdateImagesCommand, Result<ImageDto[]>>
{
    private readonly IWorkItemService _workItemService;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IFileService _fileService;

    public UpdateImagesHandler
    (
        IWorkItemService workItemService,
        IAuthenticatedUserService authenticatedUserService,
        DomainEventStore domainEventStore,
        IRepository context,
        DomainEventHandler domainEventHandler,
        IntegrationEventHandler integrationEventHandler,
        IFileService fileService
    ) : base(domainEventStore, context, domainEventHandler, integrationEventHandler)
    {
        _workItemService = workItemService;
        _authenticatedUserService = authenticatedUserService;
        _fileService = fileService;
    }

    protected override async Task<TransactionalCommandResult<Result<ImageDto[]>>> HandleAsync(ISparseRepository context, UpdateImagesCommand command, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var user = await context.DomainUsersQuery.SingleAsync(u => u.Id == userId, cancellationToken);

        var uploadedFileUrls = new List<string>();
        try
        {
            var imagesToDelete = user.Images.Where(f => !command.FilesToKeep.Any(k => k.Id == f.Id)).ToList();
            foreach (var image in imagesToDelete)
            {
                await _fileService.DeleteFileByNameAsync(image.ImageDetails.Name, cancellationToken);
                user.RemoveImage(userId, image.Id);
            }

            foreach (var (Id, Order) in command.FilesToKeep)
            {
                Console.WriteLine($"Updating order for image with Id: {Id} to Order: {Order}");
                user.ChangeImageOrder(userId, Id, Order);
            }

            foreach (var file in command.FilesToAdd)
            {
                var uploadFileResult = await _fileService.UploadFileAsync(file.Url.DataUrlToBytes(), file.MimeType, cancellationToken);
                user.AddImage(userId, new Domain.ValueObjects.ImageDetails(uploadFileResult.Name, file.MimeType, file.Order));
                uploadedFileUrls.Add(uploadFileResult.Url);
            }

            var fileUrlGenerator = new FileUrlGenerator(_fileService, TimeSpan.FromMinutes(15));
            var fileDtos = await Task.WhenAll(user.Images.Select(f => f.ToDtoAsync(fileUrlGenerator, cancellationToken)));
            return new TransactionalCommandResult<Result<ImageDto[]>>(true, Result<ImageDto[]>.Success(fileDtos));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            foreach (var url in uploadedFileUrls)
            {
                try
                {
                    await _fileService.DeleteFileByUrlAsync(url, CancellationToken.None);
                }
                catch (Exception)
                {
                    await _workItemService.PerformAsync(new DeleteFile(url), CancellationToken.None);
                }
            }
            return new TransactionalCommandResult<Result<ImageDto[]>>(false, Result<ImageDto[]>.Failure(FailureReason.Unknown, "Failed to update images."));
        }
    }
}