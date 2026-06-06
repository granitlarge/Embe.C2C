using Embe.C2C.Application.Abstractions;
using Embe.C2C.Application.Abstractions.Repos;
using Embe.C2C.Domain.Aggregates.Users;
using Microsoft.EntityFrameworkCore;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetUserByIdHandler(IC2CContext context)
{
    private readonly IC2CContext _context = context;

    public async Task<Result<User>> HandleAsync(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.DomainUsers.AsNoTracking().SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
        if (user is null)
            return Result<User>.Failure(FailureReason.NotFound, "User not found.");
        return Result<User>.Success(user);
    }
}