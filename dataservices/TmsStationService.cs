using System.Text.Json;

public static class TmsStationService
{
    public static async Task<List<Device>> GetTmsStationsAsync()
    {
        var query = @"
        query GetAllTmsStations { 
          tmsStations {
            tmsStationId
            name
            lat
            lon
            collectionStatus
            measuredTime
            sensorValues {
              name
              sensorValue
              sensorUnit
            }
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

        var root = JsonSerializer.Deserialize<TmsRoot>(result, options);

        var parsedTmsStations = root?.Data?.TmsStations?
        .Where(c => MetaCityAreaService.GetAreaService(c.Lat, c.Lon))
        .ToList() ?? new List<TmsStation>();

        var TmsStations = new List<Device>();

        if (parsedTmsStations != null)
        {
            foreach (var station in parsedTmsStations)
            {
                var deviceLocation = new Location
                {
                    lat = station.Lat,
                    lng = station.Lon,
                    elevation = 3.0
                };

                var deviceStatus = station.CollectionStatus == "GATHERING" ? "Online" : station.CollectionStatus == "REMOVED_TEMPORARILY" ? "Maintenance" : "Offline";

                var deviceId = IdIndexService.GetId();

                var newDevice = new Device
                {
                    id = deviceId,
                    name = station.TmsStationId,
                    crsType = "EPSG:4326",
                    iconName = "car",
                    location = deviceLocation,
                    status = deviceStatus,
                    sensorType = "LAM-Point",
                    description = station.Name,
                    isDataSecret = false,
                    dataLink = "https://wp.oulunliikenne.fi/avoin-data/autoliikenne/graphql-rajapinnat/#lam-pisteet",
                    measuringDirection = [-180, 180],
                    measuringRadius = 10,
                    measuringInterval = 300,
                    stationary = true,
                    dataLatestValue = null,
                };
                TmsStations.Add(newDevice);
            }
        }

        return TmsStations;
    }
}