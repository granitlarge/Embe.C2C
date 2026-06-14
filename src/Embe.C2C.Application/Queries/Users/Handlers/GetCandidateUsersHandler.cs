using Embe.C2C.Application.Abstractions;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Queries.Users.Handlers;

public class GetCandidateUsersHandler()
{
    public async Task<Result<List<User>>> HandleAsync(GetCandidateUsersQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}