using System.Text.Json;
using fs_2025_assignment_1_74780.Models;
using Microsoft.Extensions.Caching.Memory;

namespace fs_2025_assignment_1_74780.Services;

public class DublinBikeServiceV1 : IDublinBikeService
{
    private readonly IMemoryCache _cache;
    private readonly string _jsonPath;
    private List<DublinBikeStation> _stations = new();

    public DublinBikeServiceV1(IWebHostEnvironment env, IMemoryCache cache)
    {
        _cache = cache;
        _jsonPath = Path.Combine(env.ContentRootPath, "Data", "dublinbike.json");
        LoadFromFile();
    }

    private void LoadFromFile()
    {
        var json = File.ReadAllText(_jsonPath);
        _stations = JsonSerializer.Deserialize<List<DublinBikeStation>>(json)!;
    }

    public async Task<IReadOnlyList<DublinBikeStation>> GetStationsAsync(
        DublinBikeQueryOptions o,
        CancellationToken ct = default)
    {
        return await _cache.GetOrCreateAsync($"v1_stations_{o.Status}_{o.MinBikes}_{o.Search}_{o.Sort}_{o.Dir}_{o.Page}_{o.PageSize}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                IEnumerable<DublinBikeStation> query = _stations;

                if (!string.IsNullOrWhiteSpace(o.Status))
                    query = query.Where(s => s.Status.Equals(o.Status, StringComparison.OrdinalIgnoreCase));

                if (o.MinBikes != null)
                    query = query.Where(s => s.AvailableBikes >= o.MinBikes);

                if (!string.IsNullOrWhiteSpace(o.Search))
                {
                    var t = o.Search.ToLower();
                    query = query.Where(s =>
                        s.Name.ToLower().Contains(t) ||
                        s.Address.ToLower().Contains(t));
                }

                string sort = o.Sort?.ToLower() ?? "name";
                bool desc = (o.Dir?.ToLower() ?? "asc") == "desc";

                query = sort switch
                {
                    "availablebikes" => desc ? query.OrderByDescending(s => s.AvailableBikes) : query.OrderBy(s => s.AvailableBikes),
                    "occupancy" => desc ? query.OrderByDescending(s => s.Occupancy) : query.OrderBy(s => s.Occupancy),
                    _ => desc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name)
                };

                int page = Math.Max(1, o.Page ?? 1);
                int pageSize = Math.Max(1, o.PageSize ?? 20);

                query = query.Skip((page - 1) * pageSize).Take(pageSize);

                return query.ToList().AsReadOnly();
            });
    }

    public Task<DublinBikeStation?> GetByNumberAsync(int number, CancellationToken ct = default)
    {
        var station = _stations.FirstOrDefault(s => s.Number == number);
        return Task.FromResult(station);
    }

    public Task<DublinBikeSummary> GetSummaryAsync(CancellationToken ct = default)
    {
        var summary = new DublinBikeSummary
        {
            TotalStations = _stations.Count,
            TotalBikeStands = _stations.Sum(s => s.BikeStands),
            TotalAvailableBikes = _stations.Sum(s => s.AvailableBikes),
            OpenStations = _stations.Count(s => s.Status == "OPEN"),
            ClosedStations = _stations.Count(s => s.Status == "CLOSED")
        };

        return Task.FromResult(summary);
    }

    public Task<DublinBikeStation> CreateAsync(DublinBikeStation station, CancellationToken ct = default)
    {
        station.LastUpdateEpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _stations.Add(station);
        return Task.FromResult(station);
    }

    public Task<DublinBikeStation?> UpdateAsync(int number, DublinBikeStation station, CancellationToken ct = default)
    {
        var existing = _stations.FirstOrDefault(s => s.Number == number);
        if (existing == null)
            return Task.FromResult<DublinBikeStation?>(null);

        station.Number = number;
        station.LastUpdateEpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        _stations.Remove(existing);
        _stations.Add(station);

        return Task.FromResult<DublinBikeStation?>(station);
    }

    public async Task UpdateRandomAvailabilityAsync(CancellationToken ct = default)
    {
        var rnd = new Random();
        foreach (var s in _stations)
        {
            int stands = rnd.Next(10, 50);
            int bikes = rnd.Next(0, stands);

            s.BikeStands = stands;
            s.AvailableBikes = bikes;
            s.AvailableBikeStands = stands - bikes;
            s.LastUpdateEpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        await Task.CompletedTask;
    }
}