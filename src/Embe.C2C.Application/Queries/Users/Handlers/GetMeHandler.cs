using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Application.Authorizations;
using Embe.C2C.Application.Dtos.Read;
using Embe.C2C.Application.Dtos.Read.Aggregates;
using Embe.C2C.Application.Errors;
using ErrorOr;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetMeHandler : TransactionalQueryHandler<GetMeQuery, ErrorOr<ReadDto<UserDto, UserPermission>?>>
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly UserDtoMapper _userDtoMapper;

    public GetMeHandler
    (
        IUserRepository userRepo,
        IRepository repository,
        IAuthenticatedUserService authenticatedUserService,
        UserDtoMapper userDtoMapper
    ) : base(repository)
    {
        _authenticatedUserService = authenticatedUserService;
        _userDtoMapper = userDtoMapper;
        _userRepo = userRepo;
    }

    protected override async Task<ErrorOr<ReadDto<UserDto, UserPermission>?>> ExecuteAsync(GetMeQuery query, CancellationToken cancellationToken = default)
    {
        var userId = _authenticatedUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var user = await _userRepo.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ApplicationErrors.NotFound.ToNotFoundErrorOr();
        }

        var readDto = await _userDtoMapper.ToDtoAsync(user, user, cancellationToken);
        return readDto;
    }
}