namespace DublinBikes.BlazorApp.Models;

public class GeoPositionDto
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class StationDto
{
    public string Id { get; set; } = "";
    public int Number { get; set; }
    public string ContractName { get; set; } = "";
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public GeoPositionDto Position { get; set; } = new GeoPositionDto();
    public bool Banking { get; set; }
    public bool Bonus { get; set; }
    public int BikeStands { get; set; }
    public int AvailableBikeStands { get; set; }
    public int AvailableBikes { get; set; }
    public string Status { get; set; } = "OPEN";
    public long LastUpdateEpochMs { get; set; }

    public int AvailableStands
    {
        get => AvailableBikeStands;
        set => AvailableBikeStands = value;
    }

    public long LastUpdate
    {
        get => LastUpdateEpochMs;
        set => LastUpdateEpochMs = value;
    }

    public double Latitude
    {
        get => Position?.Lat ?? 0;
        set
        {
            Position ??= new GeoPositionDto();
            Position.Lat = value;
        }
    }

    public double Longitude
    {
        get => Position?.Lng ?? 0;
        set
        {
            Position ??= new GeoPositionDto();
            Position.Lng = value;
        }
    }
}