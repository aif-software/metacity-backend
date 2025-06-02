public class BikeRoot
{
    public BikeStationResponse Data { get; set; } = new();
}

public class BikeStationResponse
{
    public List<BikeRentalStation> BikeRentalStations { get; set; } = new();
}

public class BikeRentalStation
{
    public required string Id { get; set; } = string.Empty;
    public required string StationId { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public required int BikesAvailable { get; set; }
    public required int SpacesAvailable { get; set; }
    public required string State { get; set; }
    public required bool AllowDropOff { get; set; }
    public required double Lat { get; set; }
    public required double Lon { get; set; }
}
