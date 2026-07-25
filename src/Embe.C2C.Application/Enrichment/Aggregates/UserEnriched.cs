using Embe.C2C.Domain.Aggregates.Users;
using Embe.C2C.Domain.Errors.Aggregates;

namespace Embe.C2C.Application.Enrichment.Aggregates;

public record UserEnriched
(
    User User,
    double? DistanceKmToQueryingUser
);