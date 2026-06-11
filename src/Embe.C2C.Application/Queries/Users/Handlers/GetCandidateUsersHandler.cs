using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Users.Handlers
{
    public class GetCandidateUsersHandler(IRepository context, IAuthenticatedUserService userService)
    {
        private readonly IRepository _context = context;
        private readonly IAuthenticatedUserService _userService = userService;

        public async Task<Result<List<User>>> HandleAsync(GetCandidateUsersQuery request, CancellationToken cancellationToken)
        {
            var userId = _userService.UserId ?? throw new InvalidOperationException("User ID is not available.");
            var candidates = await (await _context.GetCandidatesForUserIdAsync(userId)).ToListAsync(cancellationToken);
            return Result<List<User>>.Success([.. candidates]);
        }
    }
}