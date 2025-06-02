public class TmsRoot
{
    public TmsStationResponse Data { get; set; } = new();
}

public class TmsStationResponse
{
    public List<TmsStation> TmsStations { get; set; } = new();
}

public class TmsStation
{
    public required string TmsStationId { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public required double Lat { get; set; }
    public required double Lon { get; set; }
    public required string CollectionStatus { get; set; }
    public required string MeasuredTime { get; set; }
    public required List<TmsValues> SensorValues { get; set; }
}
public class TmsValues
{
    public string Name { get; set; } = string.Empty;
    public double SensorValue { get; set; }
    public string? SensorUnit { get; set; }
}
