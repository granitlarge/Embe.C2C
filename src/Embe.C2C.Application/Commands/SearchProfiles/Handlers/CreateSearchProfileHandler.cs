using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Authorizations.FactStores.SearchProfiles;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Embe.C2C.Domain.Aggregates.SearchProfiles;
using Embe.C2C.Domain.Exceptions;
using Embe.C2C.Domain.ValueObjects;
using Embe.C2C.Domain.ValueObjects.Engagements;

namespace Embe.C2C.Application.Commands.SearchProfiles.Handlers;

public class CreateSearchProfileHandler
(
    IAuthenticatedUserService authenticatedUserService,
    DomainEventStore domainEventStore,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    SearchProfileAuthorizationService searchProfileAuthorizationService,
    SearchProfileDtoMapper searchProfileDtoMapper,
    SearchProfileAuthorizationFactStore searchProfileAuthorizationFactStore

) : TransactionalCommandHandler<CreateSearchProfileCommand, Result<ReadDto<SearchProfileDto, SearchProfilePermission>>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly SearchProfileAuthorizationService _searchProfileAuthorizationService = searchProfileAuthorizationService;
    private readonly SearchProfileDtoMapper _searchProfileDtoMapper = searchProfileDtoMapper;
    private readonly SearchProfileAuthorizationFactStore _searchProfileAuthorizationFactStore = searchProfileAuthorizationFactStore;

    protected async override Task<TransactionalCommandResult<Result<ReadDto<SearchProfileDto, SearchProfilePermission>>>> HandleAsync
    (
        ISparseRepository context,
        CreateSearchProfileCommand command,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var engagement = new Engagement
            (
                command.Engagement.Medium,
                command.Engagement.Boundedness,
                command.Engagement.Frequency,
                command.Engagement.StartDate,
                command.Engagement.EndDate
            );

            var genders = command.Genders.Count == 0 ? [.. Enum.GetValues<Gender>()] : command.Genders;
            var ageRangeMin = command.AgeRangeMin is not null ? new Age(command.AgeRangeMin.Value) : null;
            var ageRangeMax = command.AgeRangeMax is not null ? new Age(command.AgeRangeMax.Value) : null;

            var searchProfile = SearchProfile.Create
            (
                _authenticatedUserService.UserId ?? throw new InvalidOperationException("User must be authenticated"),
                command.Name,
                command.Description,
                command.RelationshipType,
                engagement,
                genders,
                ageRangeMin,
                ageRangeMax,
                command.MaximumDistanceKm is not null ? new Distance(command.MaximumDistanceKm.Value, LengthUnit.Kilometers) : null
            );

            context.SearchProfiles.Add(searchProfile);
            await context.SaveChangesAsync(cancellationToken);

            var dto = await searchProfile.ToDtoAsync(_searchProfileAuthorizationService, _searchProfileDtoMapper, cancellationToken);
            if (dto is null)
            {
                return new TransactionalCommandResult<Result<ReadDto<SearchProfileDto, SearchProfilePermission>>>
                (
                    CommitChanges: false,
                    Result<ReadDto<SearchProfileDto, SearchProfilePermission>>.Failure
                    (
                        FailureReason.Forbidden,
                        "User does not have permission to view the created search profile."
                    )
                );
            }

            var result = Result<ReadDto<SearchProfileDto, SearchProfilePermission>>.Success(dto);
            return new TransactionalCommandResult<Result<ReadDto<SearchProfileDto, SearchProfilePermission>>>
            (
                CommitChanges: true,
                Result<ReadDto<SearchProfileDto, SearchProfilePermission>>.Success(dto)
            );
        }
        catch (DomainException de)
        {
            return new TransactionalCommandResult<Result<ReadDto<SearchProfileDto, SearchProfilePermission>>>
            (
                CommitChanges: false,
                Result<ReadDto<SearchProfileDto, SearchProfilePermission>>.Failure
                (
                    FailureReason.DomainError,
                    de.Message
                )
            );
        }
    }
}