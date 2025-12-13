using fs_2025_assignment_1_74780.Models;
using fs_2025_assignment_1_74780.Services;

namespace fs_2025_assignment_1_74780.Endpoints;

public static class DublinBikeEndpoints
{
    public static IEndpointRouteBuilder MapDublinBikeEndpoints(this IEndpointRouteBuilder app)
    {
        var v1 = app.MapGroup("/api/v1/stations");
        var v2 = app.MapGroup("/api/v2/stations");

        v1.MapGet("/", async ([AsParameters] DublinBikeQueryOptions options, IDublinBikeService service, CancellationToken ct) =>
        {
            var data = await service.GetStationsAsync(options, ct);
            return Results.Ok(data);
        });

        v1.MapGet("/{number:int}", async (int number, IDublinBikeService service, CancellationToken ct) =>
        {
            var s = await service.GetByNumberAsync(number, ct);
            return s is null ? Results.NotFound() : Results.Ok(s);
        });

        v1.MapGet("/summary", async (IDublinBikeService service, CancellationToken ct) =>
        {
            var summary = await service.GetSummaryAsync(ct);
            return Results.Ok(summary);
        });

        v1.MapPost("/", async (DublinBikeStation station, IDublinBikeService service, CancellationToken ct) =>
        {
            var created = await service.CreateAsync(station, ct);
            return Results.Created($"/api/v1/stations/{created.Number}", created);
        });

        v1.MapPut("/{number:int}", async (int number, DublinBikeStation station, IDublinBikeService service, CancellationToken ct) =>
        {
            var updated = await service.UpdateAsync(number, station, ct);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        v1.MapDelete("/{number:int}", async (int number, IDublinBikeService service, CancellationToken ct) =>
        {
            var ok = await service.DeleteAsync(number, ct);
            return ok ? Results.NoContent() : Results.NotFound();
        });

        v2.MapGet("/", async ([AsParameters] DublinBikeQueryOptions options, IDublinBikeServiceV2 service, CancellationToken ct) =>
        {
            var data = await service.GetStationsAsync(options, ct);
            return Results.Ok(data);
        });

        v2.MapGet("/{number:int}", async (int number, IDublinBikeServiceV2 service, CancellationToken ct) =>
        {
            var s = await service.GetByNumberAsync(number, ct);
            return s is null ? Results.NotFound() : Results.Ok(s);
        });

        v2.MapGet("/summary", async (IDublinBikeServiceV2 service, CancellationToken ct) =>
        {
            var summary = await service.GetSummaryAsync(ct);
            return Results.Ok(summary);
        });

        v2.MapPost("/", async (DublinBikeStation station, IDublinBikeServiceV2 service, CancellationToken ct) =>
        {
            try
            {
                var created = await service.CreateAsync(station, ct);
                return Results.Created($"/api/v2/stations/{created.Number}", created);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
        });

        v2.MapPut("/{number:int}", async (int number, DublinBikeStation station, IDublinBikeServiceV2 service, CancellationToken ct) =>
        {
            var updated = await service.UpdateAsync(number, station, ct);
            return updated is null ? Results.NotFound() : Results.Ok(updated);
        });

        v2.MapDelete("/{number:int}", async (int number, IDublinBikeServiceV2 service, CancellationToken ct) =>
        {
            var ok = await service.DeleteAsync(number, ct);
            return ok ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}