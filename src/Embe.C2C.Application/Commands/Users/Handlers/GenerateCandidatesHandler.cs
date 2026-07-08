using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Authorizations.FactStores.Users;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.EventHandlers;
using Embe.C2C.Application.Extensions.Domain.Aggregates;
using Embe.C2C.Domain;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Commands.Users.Handlers;

public class GenerateCandidatesHandler
(
    IAuthenticatedUserService authenticatedUserService,
    UserAuthorizationService userAuthorizationPolicy,
    IRepository context,
    DomainEventHandler domainEventHandler,
    IntegrationEventHandler integrationEventHandler,
    UserAuthorizationFactStore userAuthorizationFactStore,
    DomainEventStore domainEventStore,
    UserDtoMapper userDtoMapper
) : TransactionalCommandHandler<GenerateCandidatesCommand, Result<List<ReadDto<UserDto, UserPermission>>>>
(
    domainEventStore,
    context,
    domainEventHandler,
    integrationEventHandler
)
{
    private readonly IAuthenticatedUserService _authenticatedUserService = authenticatedUserService;
    private readonly UserAuthorizationService _userAuthorizationPolicy = userAuthorizationPolicy;
    private readonly UserAuthorizationFactStore _userAuthorizationFactStore = userAuthorizationFactStore;
    private readonly UserDtoMapper _userDtoMapper = userDtoMapper;

    protected override async Task<TransactionalCommandResult<Result<List<ReadDto<UserDto, UserPermission>>>>> HandleAsync
    (
        ISparseRepository context,
        GenerateCandidatesCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var userId = _authenticatedUserService.UserId ?? throw new InvalidOperationException("User is not authenticated.");
        var queryingUser = await context.DomainUsersQuery.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        var users = await context.GenerateCandidatesForUserIdAsync(userId, cancellationToken);
        var dtos = new List<ReadDto<UserDto, UserPermission>>();
        foreach (var user in users)
        {
            _userAuthorizationFactStore.SetCandidateUserFact(user.Id, true);
            var (permissions, variant) = await _userAuthorizationPolicy.GetAsync(user.Id, cancellationToken);
            var enrichedUser = user.Enrich(queryingUser);
            var dto = await _userDtoMapper.ToDtoAsync(enrichedUser, variant, cancellationToken);
            if (dto is not null)
            {
                dtos.Add(new ReadDto<UserDto, UserPermission>(dto, permissions));
            }
        }
        var result = Result<List<ReadDto<UserDto, UserPermission>>>.Success(dtos);
        return new TransactionalCommandResult<Result<List<ReadDto<UserDto, UserPermission>>>>(true, result);
    }
}