public class Device
{
    public int? id { get; set; }
    public required string name { get; set; }
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
    public WeatherData? weatherData { get; set; }
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

public class WeatherData
{
    public double? air_temperature { get; set; }
    public double? air_relative_humidity { get; set; }
    public double? dew_point_temperature { get; set; }
    public double? wind_speed { get; set; }
    public double? wind_direction { get; set; }
    public double? rainfall_depth { get; set; }
    public double? rainfall_intensity { get; set; }
    public double? snow_depth_a { get; set; }
    public double? snow_depth_b { get; set; }
    public double? snow_depth_c { get; set; }
    public double? snow_depth { get; set; }
    public double? rain_classification { get; set; }
    public double? road_surface_temperature { get; set; }
}
