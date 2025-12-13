namespace DublinBikes.BlazorApp.Models;

public class DublinBikeRawDto
{
    public int number { get; set; }
    public string name { get; set; } = string.Empty;
    public string address { get; set; } = string.Empty;

    public Position position { get; set; } = new();

    public int bike_stands { get; set; }
    public int available_bike_stands { get; set; }
    public int available_bikes { get; set; }

    public string status { get; set; } = string.Empty;

    public long last_update { get; set; }

    public class Position
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}