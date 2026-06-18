using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Infrastructure.Ef.Entities;

public class CandidateEntity(Guid userId, Guid candidateUserId)
{
    public Guid UserId { get; } = userId;
    public Guid CandidateUserId { get; } = candidateUserId;

    public User? Candidate { get; private set; }
}