using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Enrichment.Aggregates;

public record UserEnriched
(
    User User,
    double? DistanceKmToQueryingUser
);