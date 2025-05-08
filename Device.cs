public class Device
{
    public required string id { get; set; }
    public required string crsType { get; set; }
    public string? iconName { get; set; }
    public required Location location { get; set; }
    public required string status { get; set; }
    public required string sensorType { get; set; }
    public string? sensorModel { get; set; }
    public required string description { get; set; }
    public required bool isDataSecret { get; set; }
    public string? dataLink { get; set; }
    public double[]? measuringDirection { get; set; }
    public double? measuringRadius { get; set; }
    public double? measuringInterval { get; set; }
    public string? measuringDescription { get; set; }
    public required bool stationary { get; set; }
    public string? dataLatestValue { get; set; }
}

public class Location
{
    public required double lat { get; set; }
    public required double lng { get; set; }
    public double elevation { get; set; }
    public List<LatLng>? path { get; set; }
    public List<LatLng>? area { get; set; }
}

public class LatLng
{
    public double lat { get; set; }
    public double lng { get; set; }
}