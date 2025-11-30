using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using fs_2025_assignment_1_74780;
using fs_2025_assignment_1_74780.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace fs_2025_assignment_1_74780.Tests;

public class DublinBikeEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DublinBikeEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStations_ReturnsOkAndData()
    {
        var response = await _client.GetAsync("/api/v1/stations");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stations = await response.Content.ReadFromJsonAsync<List<DublinBikeStation>>();
        Assert.NotNull(stations);
        Assert.NotEmpty(stations);
    }

    [Fact]
    public async Task GetStations_FilterByStatusOpen_ReturnsOnlyOpen()
    {
        var response = await _client.GetAsync("/api/v1/stations?status=OPEN");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stations = await response.Content.ReadFromJsonAsync<List<DublinBikeStation>>();
        Assert.NotNull(stations);
        Assert.All(stations!, s => Assert.True(s.Status.ToUpper() == "OPEN"));
    }

    [Fact]
    public async Task GetStations_MinBikesFilter_Works()
    {
        int minBikes = 5;
        var response = await _client.GetAsync($"/api/v1/stations?minBikes={minBikes}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stations = await response.Content.ReadFromJsonAsync<List<DublinBikeStation>>();
        Assert.NotNull(stations);
        Assert.All(stations!, s => Assert.True(s.AvailableBikes >= minBikes));
    }

    [Fact]
    public async Task GetStations_SortingByAvailableBikesDescending_Works()
    {
        var response = await _client.GetAsync("/api/v1/stations?sort=availableBikes&dir=desc");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stations = await response.Content.ReadFromJsonAsync<List<DublinBikeStation>>();
        Assert.NotNull(stations);
        Assert.True(stations!.Count > 1);

        var values = stations.Select(s => s.AvailableBikes).ToList();
        var sorted = values.OrderByDescending(x => x).ToList();

        Assert.Equal(sorted, values);
    }

    [Fact]
    public async Task GetStations_Paging_Works()
    {
        var r1 = await _client.GetAsync("/api/v1/stations?page=1&pageSize=5");
        var r2 = await _client.GetAsync("/api/v1/stations?page=2&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, r1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, r2.StatusCode);

        var p1 = await r1.Content.ReadFromJsonAsync<List<DublinBikeStation>>();
        var p2 = await r2.Content.ReadFromJsonAsync<List<DublinBikeStation>>();

        Assert.NotNull(p1);
        Assert.NotNull(p2);

        Assert.Equal(5, p1!.Count);
        Assert.Equal(5, p2!.Count);

        var ids1 = p1.Select(s => s.Number).ToHashSet();
        var ids2 = p2.Select(s => s.Number).ToHashSet();

        Assert.True(ids1.Intersect(ids2).Count() == 0);
    }

    [Fact]
    public async Task GetSummary_ReturnsExpectedShape()
    {
        var response = await _client.GetAsync("/api/v1/stations/summary");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var summary = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        Assert.NotNull(summary);
        Assert.True(summary!.ContainsKey("totalStations"));
        Assert.True(summary.ContainsKey("totalBikeStands"));
        Assert.True(summary.ContainsKey("totalAvailableBikes"));
    }
}