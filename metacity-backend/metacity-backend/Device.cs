public class Device
{
    public required string Id { get; set; }
    public required Location Location { get; set; }
    public required string CrsType { get; set; }
    public required string Status { get; set; }
    public string? SensorType { get; set; }
    public required string Description { get; set; }
    public required bool IsDataSecret { get; set; }
    public string? DataLink { get; set; }
    public double[]? measuringDirection { get; set; }
    public string? dataLatestValue { get; set; }
}

public class Location
{
    public required double Lat { get; set; }
    public required double Lng { get; set; }
    public double Elevation { get; set; }
    public List<LatLng>? Path { get; set; }
    public List<LatLng>? Area { get; set; }
}

public class LatLng
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}