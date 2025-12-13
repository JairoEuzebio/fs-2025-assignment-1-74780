using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DublinBikes.BlazorApp.Models;

namespace DublinBikes.BlazorApp.Services;

public class StationsApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public StationsApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<StationDto>> GetStationsAsync(
        QueryState state,
        CancellationToken ct = default)
    {
        var url = BuildStationsUrl(state);

        var res = await _http.GetAsync(url, ct);
        await EnsureOk(res);

        var data = await res.Content.ReadFromJsonAsync<List<StationDto>>(_json, ct);
        return (data ?? new List<StationDto>()).AsReadOnly();
    }

    public async Task<StationDto?> GetByNumberAsync(
        int number,
        CancellationToken ct = default)
    {
        var res = await _http.GetAsync($"/api/v2/stations/{number}", ct);

        if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        await EnsureOk(res);
        return await res.Content.ReadFromJsonAsync<StationDto>(_json, ct);
    }

    public async Task<StationDto> CreateAsync(
        StationDto station,
        CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync(
            "/api/v2/stations",
            station,
            _json,
            ct);

        await EnsureOk(res);

        var created = await res.Content.ReadFromJsonAsync<StationDto>(_json, ct);
        return created!;
    }

    public async Task<StationDto> UpdateAsync(
        int number,
        StationDto station,
        CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync(
            $"/api/v2/stations/{number}",
            station,
            _json,
            ct);

        await EnsureOk(res);

        var updated = await res.Content.ReadFromJsonAsync<StationDto>(_json, ct);
        return updated!;
    }

    public async Task DeleteAsync(
        int number,
        CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync(
            $"/api/v2/stations/{number}",
            ct);

        await EnsureOk(res);
    }

    private static string BuildStationsUrl(QueryState s)
    {
        var qs = new List<string>();

        if (!string.IsNullOrWhiteSpace(s.Search))
            qs.Add($"search={Uri.EscapeDataString(s.Search)}");

        if (!string.IsNullOrWhiteSpace(s.Status) &&
            !s.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            qs.Add($"status={Uri.EscapeDataString(s.Status)}");

        if (s.MinBikes is not null)
            qs.Add($"minBikes={s.MinBikes}");

        if (!string.IsNullOrWhiteSpace(s.SortBy))
            qs.Add($"sort={Uri.EscapeDataString(s.SortBy)}");

        qs.Add($"dir={(s.SortDesc ? "desc" : "asc")}");

        qs.Add($"page={Math.Max(1, s.Page)}");
        qs.Add($"pageSize={Math.Clamp(s.PageSize, 1, 200)}");

        return "/api/v2/stations?" + string.Join("&", qs);
    }

    private static async Task EnsureOk(HttpResponseMessage res)
    {
        if (res.IsSuccessStatusCode)
            return;

        var body = await res.Content.ReadAsStringAsync();

        var msg = new StringBuilder();
        msg.Append($"API error {(int)res.StatusCode} {res.ReasonPhrase}");

        if (!string.IsNullOrWhiteSpace(body))
        {
            msg.Append(" - ");
            msg.Append(body.Length > 1200 ? body[..1200] : body);
        }

        throw new HttpRequestException(
            msg.ToString(),
            null,
            res.StatusCode);
    }
}