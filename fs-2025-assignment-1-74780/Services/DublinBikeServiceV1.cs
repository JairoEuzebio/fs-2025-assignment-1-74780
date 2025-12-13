using fs_2025_assignment_1_74780.Models;

namespace fs_2025_assignment_1_74780.Services;

public class DublinBikeServiceV1 : IDublinBikeService
{
    public Task<IReadOnlyList<DublinBikeStation>> GetStationsAsync(DublinBikeQueryOptions options, CancellationToken ct = default)
    {
        return Task.FromResult((IReadOnlyList<DublinBikeStation>)new List<DublinBikeStation>());
    }

    public Task<DublinBikeStation?> GetByNumberAsync(int number, CancellationToken ct = default)
    {
        return Task.FromResult<DublinBikeStation?>(null);
    }

    public Task<DublinBikeSummary> GetSummaryAsync(CancellationToken ct = default)
    {
        return Task.FromResult(new DublinBikeSummary());
    }

    public Task<DublinBikeStation> CreateAsync(DublinBikeStation station, CancellationToken ct = default)
    {
        return Task.FromResult(station);
    }

    public Task<DublinBikeStation?> UpdateAsync(int number, DublinBikeStation station, CancellationToken ct = default)
    {
        return Task.FromResult<DublinBikeStation?>(station);
    }

    public Task<bool> DeleteAsync(int number, CancellationToken ct = default)
    {
        return Task.FromResult(false);
    }

    public Task UpdateRandomAvailabilityAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}