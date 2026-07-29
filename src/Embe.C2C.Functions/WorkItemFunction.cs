using Azure.Messaging.ServiceBus;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices;
using Embe.C2C.Application.Abstractions.Services.WorkItemServices.WorkItems;
using Embe.C2C.Application.Commands.SearchProfiles;
using Embe.C2C.Application.Commands.SearchProfiles.Handlers;
using Microsoft.Azure.Functions.Worker;

namespace Embe.C2C.Functions;

public class WorkItemFunction
(
    CreateSearchProfileEmbeddingHandler createSearchProfileEmbeddingHandler,
    IImageService imageService
)
{
    private readonly CreateSearchProfileEmbeddingHandler _createSearchProfileEmbeddingHandler = createSearchProfileEmbeddingHandler;
    private readonly IImageService _imageService = imageService;

    [Function(nameof(WorkItemFunction))]
    public async Task HandleAsync
    (
        [ServiceBusTrigger("work-items", Connection = "AzureServiceBus", AutoCompleteMessages = false)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions serviceBusMessageActions,
        CancellationToken cancellationToken
    )
    {
        var item = message.Body.ToObjectFromJson<IWorkItem>();
        if (item is null)
        {
            await serviceBusMessageActions.DeadLetterMessageAsync(message, cancellationToken: cancellationToken);
        }

        switch (item)
        {

            case GenerateSearchProfileDescriptionEmbedding command:
                await _createSearchProfileEmbeddingHandler.HandleAsync
                (
                    new CreateSearchProfileEmbeddingCommand(command.SearchProfileId, command.Description),
                    cancellationToken
                );
                await serviceBusMessageActions.CompleteMessageAsync(message, cancellationToken);
                break;

            case DeleteImage deleteImage:
                {
                    await _imageService.DeleteImageByUrlAsync(deleteImage.Url, cancellationToken);
                    await serviceBusMessageActions.CompleteMessageAsync(message, cancellationToken);
                    break;
                }

            default:
                throw new NotImplementedException();

        }
    }
}