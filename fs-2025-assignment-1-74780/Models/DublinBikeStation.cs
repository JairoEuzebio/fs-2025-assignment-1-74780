using System.Text.Json.Serialization;

namespace fs_2025_assignment_1_74780.Models;

public class GeoPosition
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}

public class DublinBikeStation
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("contract_name")]
    public string ContractName { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("address")]
    public string Address { get; set; } = "";

    [JsonPropertyName("position")]
    public GeoPosition Position { get; set; } = new GeoPosition();

    [JsonPropertyName("banking")]
    public bool Banking { get; set; }

    [JsonPropertyName("bonus")]
    public bool Bonus { get; set; }

    [JsonPropertyName("bike_stands")]
    public int BikeStands { get; set; }

    [JsonPropertyName("available_bike_stands")]
    public int AvailableBikeStands { get; set; }

    [JsonPropertyName("available_bikes")]
    public int AvailableBikes { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("last_update")]
    public long LastUpdateEpochMs { get; set; }

 

    public double Occupancy =>
        BikeStands == 0 ? 0 : (double)AvailableBikes / BikeStands;

    public DateTimeOffset LastUpdateUtc =>
        DateTimeOffset.FromUnixTimeMilliseconds(LastUpdateEpochMs);

    public DateTimeOffset LastUpdateDublin
    {
        get
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Dublin");
            return TimeZoneInfo.ConvertTime(LastUpdateUtc, tz);
        }
    }
}