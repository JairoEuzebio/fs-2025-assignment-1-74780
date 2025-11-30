using fs_2025_assignment_1_74780.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace fs_2025_assignment_1_74780.Services;

public class DublinBikeServiceV2 : IDublinBikeServiceV2
{
    private readonly Container _container;

    public DublinBikeServiceV2(CosmosClient client, IOptions<CosmosOptions> options)
    {
        var opt = options.Value;
        _container = client.GetDatabase(opt.DatabaseName).GetContainer(opt.ContainerName);
    }

    private async Task<List<DublinBikeStation>> LoadAllAsync(CancellationToken ct)
    {
        var results = new List<DublinBikeStation>();
        var query = _container.GetItemQueryIterator<DublinBikeStation>("SELECT * FROM c");
        while (query.HasMoreResults)
        {
            var page = await query.ReadNextAsync(ct);
            results.AddRange(page);
        }
        return results;
    }

    public async Task<IReadOnlyList<DublinBikeStation>> GetStationsAsync(DublinBikeQueryOptions o, CancellationToken ct = default)
    {
        var stations = await LoadAllAsync(ct);
        IEnumerable<DublinBikeStation> query = stations;

        if (!string.IsNullOrWhiteSpace(o.Status))
            query = query.Where(s => s.Status.Equals(o.Status, StringComparison.OrdinalIgnoreCase));

        if (o.MinBikes != null)
            query = query.Where(s => s.AvailableBikes >= o.MinBikes);

        if (!string.IsNullOrWhiteSpace(o.Search))
        {
            var t = o.Search.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(t) || s.Address.ToLower().Contains(t));
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
    }

    public async Task<DublinBikeStation?> GetByNumberAsync(int number, CancellationToken ct = default)
    {
        var query = _container.GetItemQueryIterator<DublinBikeStation>(
            new QueryDefinition("SELECT * FROM c WHERE c.number = @number").WithParameter("@number", number));

        while (query.HasMoreResults)
        {
            var page = await query.ReadNextAsync(ct);
            var station = page.Resource.FirstOrDefault();
            if (station != null)
                return station;
        }

        return null;
    }

    public async Task<DublinBikeSummary> GetSummaryAsync(CancellationToken ct = default)
    {
        var stations = await LoadAllAsync(ct);

        var summary = new DublinBikeSummary
        {
            TotalStations = stations.Count,
            TotalBikeStands = stations.Sum(s => s.BikeStands),
            TotalAvailableBikes = stations.Sum(s => s.AvailableBikes),
            OpenStations = stations.Count(s => s.Status == "OPEN"),
            ClosedStations = stations.Count(s => s.Status == "CLOSED")
        };

        return summary;
    }

    public async Task<DublinBikeStation> CreateAsync(DublinBikeStation station, CancellationToken ct = default)
    {
        station.LastUpdateEpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await _container.CreateItemAsync(station, cancellationToken: ct);
        return station;
    }

    public async Task<DublinBikeStation?> UpdateAsync(int number, DublinBikeStation station, CancellationToken ct = default)
    {
        var existing = await GetByNumberAsync(number, ct);
        if (existing == null)
            return null;

        station.Number = number;
        station.LastUpdateEpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        await _container.UpsertItemAsync(station, cancellationToken: ct);
        return station;
    }

    public async Task UpdateRandomAvailabilityAsync(CancellationToken ct = default)
    {
        var rnd = new Random();
        var stations = await LoadAllAsync(ct);

        foreach (var s in stations)
        {
            int stands = rnd.Next(10, 50);
            int bikes = rnd.Next(0, stands);

            s.BikeStands = stands;
            s.AvailableBikes = bikes;
            s.AvailableBikeStands = stands - bikes;
            s.LastUpdateEpochMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            await _container.UpsertItemAsync(s, cancellationToken: ct);
        }
    }
}