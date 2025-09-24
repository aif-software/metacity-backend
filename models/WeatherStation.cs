public class WeatherRoot
{
    public CityWeatherStationsResponse Data { get; set; } = new();
}

public class CityWeatherStationsResponse
{
    public List<CityWeatherStation> CityWeatherStations { get; set; } = new();
}

public class CityWeatherStation
{
    public string WeatherStationId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public List<Values> SensorValues { get; set; } = new();
    public List<Camera> Cameras { get; set; } = new();
}

public class Values
{
    public string Name { get; set; } = string.Empty;
    public string SensorId { get; set; } = string.Empty;
    public double? SensorValue { get; set; }
    public string? SensorUnit { get; set; }
    public DateTime MeasuredTime { get; set; }
}

public class Camera
{
    public int CameraId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
