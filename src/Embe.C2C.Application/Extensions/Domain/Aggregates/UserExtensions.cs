using Embe.C2C.Application.Enrichment.Aggregates;
using Embe.C2C.Domain.Aggregates.Users;

namespace Embe.C2C.Application.Extensions.Domain.Aggregates;

public static class UserExtensions
{
    public static UserEnriched Enrich(this User user, User? queryingUser)
    {
        var distanceKm = user.Location != null && queryingUser?.Location != null
            ? user.Location.DistanceTo(queryingUser.Location).ToKilometers().Value
            : (double?)null;
        return new UserEnriched(user, distanceKm);
    }
}