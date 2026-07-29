using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Domain;
using ErrorOr;

namespace Embe.C2C.Application.Commands.SearchProfiles.Handlers;

public class CreateSearchProfileEmbeddingHandler
(
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    ISemanticEmbeddingService semanticEmbeddingService,
    ISearchProfileRepository searchProfileRepository
) : CommandHandler<CreateSearchProfileEmbeddingCommand, ErrorOr<Success>>(domainEventStore, context, domainEventHandler, integrationEventHandler)
{

    private readonly ISemanticEmbeddingService _semanticEmbeddingService = semanticEmbeddingService;
    private readonly ISearchProfileRepository _searchProfileRepository = searchProfileRepository;

    protected override async Task<CommandResult<ErrorOr<Success>>> InternalHandleAsync
    (
        CreateSearchProfileEmbeddingCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var embedding = await _semanticEmbeddingService.GetAsync(command.Content, cancellationToken);
        await _searchProfileRepository.StoreEmbeddingAsync(command.SearchProfileId, embedding, cancellationToken);
        return new(true, Result.Success);
    }

}