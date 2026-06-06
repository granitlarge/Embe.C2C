using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Application.Abstractions.Services;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Users.Handlers
{
    public class GetCandidateUsersHandler(IC2CContext context, IAuthenticatedUserService userService)
    {
        private readonly IC2CContext _context = context;
        private readonly IAuthenticatedUserService _userService = userService;

        public async Task<Result<List<User>>> HandleAsync(GetCandidateUsersQuery request, CancellationToken cancellationToken)
        {
            var userId = _userService.UserId ?? throw new InvalidOperationException("User ID is not available.");

            using var transaction = await _context.BeginTransactionAsync();
            var user = await _context.DomainUsers.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null)
                return Result<List<User>>.Failure(FailureReason.NotFound, "User not found.");
            throw new NotImplementedException();
        }
    }
}