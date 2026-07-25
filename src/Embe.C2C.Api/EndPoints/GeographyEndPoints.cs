using Embe.C2C.Api.Extensions;
using Embe.C2C.Application.Queries.Geography;
using Embe.C2C.Application.Queries.Geography.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Embe.C2C.Api.EndPoints;

public static class GeographyEndpoints
{
    public static void MapGeographyEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/geography");
        group.MapGet("", SearchAdminAreas);
        group.MapGet("/{id}", GetAdminAreaById);
        group.MapGet("/country", GetCountryAdminAreas);
        group.MapPost("/reverse-geocode", ReverseGeocode);
    }

    private static async Task<IResult> GetCountryAdminAreas([FromServices] GetCountryAdminAreaHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> SearchAdminAreas
    (
        [FromQuery] string? parentId,
        [FromQuery] double? longitude,
        [FromQuery] double? latitude,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromServices] SearchAdminAreaHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(new SearchAdminAreaQuery(parentId, longitude, latitude, page ?? 1, pageSize ?? 50), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAdminAreaById([FromRoute] string id, [FromServices] GetAdminAreaByIdHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetAdminAreaByIdQuery(id), cancellationToken);
        return Results.Ok(result);
    }

    public record ReverseGeocodeRequest(double Longitude, double Latitude);
    private static async Task<IResult> ReverseGeocode
    (
        [FromBody] ReverseGeocodeRequest request,
        [FromServices] ReverseGeocodeHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(new ReverseGeocodeQuery(request.Longitude, request.Latitude), cancellationToken);
        return Results.Ok(result);
    }

}