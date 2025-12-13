using fs_2025_assignment_1_74780.Models;

namespace fs_2025_assignment_1_74780.Services;

public class DublinBikeQueryOptions
{
    public string? Status { get; set; }
    public int? MinBikes { get; set; }
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public string? Dir { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class DublinBikeSummary
{
    public int TotalStations { get; set; }
    public int TotalBikeStands { get; set; }
    public int TotalAvailableBikes { get; set; }
    public int OpenStations { get; set; }
    public int ClosedStations { get; set; }
}

public interface IDublinBikeService
{
    Task<IReadOnlyList<DublinBikeStation>> GetStationsAsync(DublinBikeQueryOptions options, CancellationToken ct = default);
    Task<DublinBikeStation?> GetByNumberAsync(int number, CancellationToken ct = default);
    Task<DublinBikeSummary> GetSummaryAsync(CancellationToken ct = default);
    Task<DublinBikeStation> CreateAsync(DublinBikeStation station, CancellationToken ct = default);
    Task<DublinBikeStation?> UpdateAsync(int number, DublinBikeStation station, CancellationToken ct = default);
    Task<bool> DeleteAsync(int number, CancellationToken ct = default);
    Task UpdateRandomAvailabilityAsync(CancellationToken ct = default);
}

public interface IDublinBikeServiceV2 : IDublinBikeService
{
}