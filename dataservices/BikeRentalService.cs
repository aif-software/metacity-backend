using System.Text.Json;

public static class BikeStationService
{
    public static async Task<List<Device>> GetBikeStationsAsync()
    {
        var query = @"
        query GetAllBikeRentalStations {
          bikeRentalStations {
            id
            stationId
            name
            bikesAvailable
            spacesAvailable
            state
            allowDropoff
            lat
            lon
          }
        }
        ";

        var client = new HttpClient();
        var graphQLClient = new GraphQLClient(client);

        string endpoint = "https://api.oulunliikenne.fi/proxy/graphql";
        string result = await graphQLClient.SendQueryAsync(endpoint, query);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var root = JsonSerializer.Deserialize<BikeRoot>(result, options);

        var parsedBikeStations = root?.Data?.BikeRentalStations?
        .Where(c => MetaCityAreaService.GetAreaService(c.Lat, c.Lon))
        .ToList() ?? new List<BikeRentalStation>();

        var bikeStations = new List<Device>();

        if (parsedBikeStations != null)
        {
            foreach (var station in parsedBikeStations)
            {
                var deviceLocation = new Location
                {
                    lat = station.Lat,
                    lng = station.Lon,
                    elevation = 1.0
                };

                var deviceId = IdIndexService.GetId();

                var newDevice = new Device
                {
                    id = deviceId,
                    name = station.Name,
                    crsType = "EPSG:4326",
                    iconName = "bike",
                    location = deviceLocation,
                    status = "Online",
                    sensorType = "Bike Rental",
                    description = station.StationId,
                    isDataSecret = false,
                    dataLink = "https://wp.oulunliikenne.fi/avoin-data/pyorailykavely/graphql-rajapinnat/#kaupunkipy%C3%B6r%C3%A4asemat",
                    measuringDirection = [-180, 180],
                    measuringRadius = 10,
                    measuringInterval = 300,
                    stationary = true,
                    dataLatestValue = null,
                };

                bikeStations.Add(newDevice);
            }
        }

        return bikeStations;
    }
}
