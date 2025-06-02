using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class EcoCounterService
{
    public static async Task<List<Device>> GetEcoCountersAsync()
    {
        var query = @"
        query GetAllEcoCounterSites {
          ecoCounterSites {
            id
            siteId
            name
            domain
            userType
            timezone
            interval
            sens
            channels {
              id
              siteId
              name
              domain
              userType
              timezone
              interval
              sens
              lat
              lon
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

        var parsed = JsonSerializer.Deserialize<GraphQLResponse<EcoCounterSitesData>>(result, options);

        if (parsed == null)
        {
            Console.WriteLine("parsed is null");
        }
        else if (parsed.Data == null)
        {
            Console.WriteLine("parsed.Data is null");
        }
        else if (parsed.Data.EcoCounterSites == null)
        {
            Console.WriteLine("parsed.Data.EcoCounterSites is null");
        }


        var allChannels = parsed?.Data?.EcoCounterSites?
        .Where(site => site?.Channels != null)
        .SelectMany(site => site.Channels)
        .Where(c => MetaCityAreaService.GetAreaService(c.Lat, c.Lon))
        .ToList() ?? new List<Channel>();

        var counterDevices = new List<Device>();

        foreach (var channel in allChannels)
        {
            var deviceLocation = new Location
            {
                lat = channel.Lat,
                lng = channel.Lon,
                elevation = 1.0
            };

            var deviceCountertype = channel.UserType == 1 ? "pedestrian counter" : channel.UserType == 2 ? "bicycle counter" : "";
            var deviceIcon = channel.UserType == 1 ? "motion" : channel.UserType == 2 ? "bike" : "";

            var deviceId = IdIndexService.GetId();

            var newDevice = new Device
            {
                id = deviceId,
                name = channel.Name,
                crsType = "EPSG:4326",
                iconName = deviceIcon,
                location = deviceLocation,
                status = "Online",
                sensorType = deviceCountertype,
                description = channel.SiteId,
                isDataSecret = false,
                dataLink = "https://wp.oulunliikenne.fi/avoin-data/pyorailykavely/graphql-rajapinnat/#py%C3%B6r%C3%A4ily-ja-k%C3%A4vely-m%C3%A4%C3%A4r%C3%A4t",
                measuringDirection = [-180, 180],
                measuringRadius = 10,
                measuringInterval = channel.Interval,
                measuringDescription = deviceCountertype,
                stationary = true,
                dataLatestValue = null,
            };

            counterDevices.Add(newDevice);
        }

        return counterDevices;
    }
}
