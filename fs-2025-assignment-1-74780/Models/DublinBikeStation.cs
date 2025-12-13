using Newtonsoft.Json;

namespace fs_2025_assignment_1_74780.Models;

public class GeoPosition
{
    [JsonProperty("lat")]
    public double Lat { get; set; }

    [JsonProperty("lng")]
    public double Lng { get; set; }
}

public class DublinBikeStation
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("number")]
    public int Number { get; set; }

    [JsonProperty("contract_name")]
    public string ContractName { get; set; } = "";

    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("address")]
    public string Address { get; set; } = "";

    [JsonProperty("position")]
    public GeoPosition Position { get; set; } = new();

    [JsonProperty("banking")]
    public bool Banking { get; set; }

    [JsonProperty("bonus")]
    public bool Bonus { get; set; }

    [JsonProperty("bike_stands")]
    public int BikeStands { get; set; }

    [JsonProperty("available_bike_stands")]
    public int AvailableBikeStands { get; set; }

    [JsonProperty("available_bikes")]
    public int AvailableBikes { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = "";

    [JsonProperty("last_update")]
    public long LastUpdateEpochMs { get; set; }

    [JsonIgnore]
    public double Occupancy => BikeStands == 0 ? 0 : (double)AvailableBikes / BikeStands;

    [JsonIgnore]
    public DateTimeOffset LastUpdateUtc => DateTimeOffset.FromUnixTimeMilliseconds(LastUpdateEpochMs);

    [JsonIgnore]
    public DateTimeOffset LastUpdateDublin
    {
        get
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Dublin");
            return TimeZoneInfo.ConvertTime(LastUpdateUtc, tz);
        }
    }
}