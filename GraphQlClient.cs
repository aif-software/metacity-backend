using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class GraphQLRequest
{
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    [JsonPropertyName("variables")]
    public object? Variables { get; set; } = null;
}

public class GraphQLClient
{
    private readonly HttpClient _httpClient;

    public GraphQLClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Device>> SendQueryAsync(string endpoint, string query, object? variables = null)
    {
        var request = new GraphQLRequest
        {
            Query = query,
            Variables = variables
        };

        var jsonRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var parsed = JsonSerializer.Deserialize<GraphQLResponse<EcoCounterSitesData>>(responseBody, options);

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
            .Where(c => c.Lat >= 65.04860552362892 && c.Lat <= 65.07252801487695 && c.Lon >= 25.4298734664917 && c.Lon <= 25.519523620605472)
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

            var newDevice = new Device
            {
                id = channel.Id,
                crsType = "EPSG:4326",
                iconName = "motion",
                location = deviceLocation,
                status = "Online",
                sensorType = "Counter",
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

/*

{
    "id": "OULU176_PP_A29_A30__A",
    "crsType": "EPSG:4326",
    "iconName": "motion",
    "location": { "lat": 65.067715, "lng": 25.494297, "elevation": 0 },
    "status": "Online",
    "sensorType": "Counter",
    "description": "PP_A29_A30__A",
    "IsDataSecret": false,
    "measuringDirection": [340, 20],
    "measuringRadius": 10,
    "measuringInterval": 60,
    "measuringDescription": "",
    "stationary": true,
    "dataLink": "https://wp.oulunliikenne.fi/avoin-data/pyorailykavely/graphql-rajapinnat/#py%C3%B6r%C3%A4ily-ja-k%C3%A4vely-m%C3%A4%C3%A4r%C3%A4t",
    "dataLatestValue": null
  },
  {
    "id": "OULU176_PP_B29_B30__B",
    "crsType": "EPSG:4326",
    "iconName": "motion",
    "location": { "lat": 65.067715, "lng": 25.495297, "elevation": 0 },
    "status": "Online",
    "sensorType": "Counter",
    "description": "PP_B29_B30__B",
    "IsDataSecret": false,
    "measuringDirection": [350, 30],
    "measuringRadius": 10,
    "measuringInterval": 60,
    "measuringDescription": "",
    "stationary": true,
    "dataLink": "https://wp.oulunliikenne.fi/avoin-data/pyorailykavely/graphql-rajapinnat/#py%C3%B6r%C3%A4ily-ja-k%C3%A4vely-m%C3%A4%C3%A4r%C3%A4t",
    "dataLatestValue": null
  },
  {
    "id": "OULU175_pp_a29_a30",
    "crsType": "EPSG:4326",
    "iconName": "motion",
    "location": { "lat": 65.065897, "lng": 25.503838, "elevation": 0 },
    "status": "Online",
    "sensorType": "Counter",
    "description": "pp_a29_a30",
    "IsDataSecret": false,
    "measuringDirection": [0, 40],
    "measuringRadius": 10,
    "measuringInterval": 60,
    "measuringDescription": "",
    "stationary": true,
    "dataLink": "https://wp.oulunliikenne.fi/avoin-data/pyorailykavely/graphql-rajapinnat/#py%C3%B6r%C3%A4ily-ja-k%C3%A4vely-m%C3%A4%C3%A4r%C3%A4t",
    "dataLatestValue": null
  },
  {
    "id": "OULU175_pp_b29_b30",
    "crsType": "EPSG:4326",
    "iconName": "motion",
    "location": { "lat": 65.065897, "lng": 25.5048, "elevation": 0 },
    "status": "Online",
    "sensorType": "Counter",
    "description": "pp_b29_b30",
    "IsDataSecret": false,
    "measuringDirection": [0, 50],
    "measuringRadius": 10,
    "measuringInterval": 60,
    "measuringDescription": "",
    "stationary": true,
    "dataLink": "https://wp.oulunliikenne.fi/avoin-data/pyorailykavely/graphql-rajapinnat/#py%C3%B6r%C3%A4ily-ja-k%C3%A4vely-m%C3%A4%C3%A4r%C3%A4t",
    "dataLatestValue": null
  },
  {
    "id": "OULU174_PP1_PP2",
    "crsType": "EPSG:4326",
    "iconName": "motion",
    "location": { "lat": 65.062924, "lng": 25.509751, "elevation": 0 },
    "status": "Online",
    "sensorType": "Counter",
    "description": "PP1_PP2",
    "IsDataSecret": false,
    "measuringDirection": [0, 60],
    "measuringRadius": 10,
    "measuringInterval": 60,
    "measuringDescription": "",
    "stationary": true,
    "dataLink": null,
    "dataLatestValue": null
  },
  {
    "id": "OULU174_PP2_PP1",
    "crsType": "EPSG:4326",
    "iconName": "motion",
    "location": { "lat": 65.062924, "lng": 25.510751, "elevation": 0 },
    "status": "Online",
    "sensorType": "Counter",
    "description": "PP2_PP1",
    "IsDataSecret": false,
    "measuringDirection": [0, 70],
    "measuringRadius": 10,
    "measuringInterval": 60,
    "measuringDescription": "",
    "stationary": true,
    "dataLink": "https://wp.oulunliikenne.fi/avoin-data/pyorailykavely/graphql-rajapinnat/#py%C3%B6r%C3%A4ily-ja-k%C3%A4vely-m%C3%A4%C3%A4r%C3%A4t",
    "dataLatestValue": null
  },
  {
    "id": "OULU174_PP5_PP6",
    "crsType": "EPSG:4326",
    "iconName": "motion",
    "location": { "lat": 65.062701, "lng": 25.513529, "elevation": 0 },
    "status": "Online",
    "sensorType": "Counter",
    "description": "PP5_PP6",
    "IsDataSecret": false,
    "measuringDirection": [0, 80],
    "measuringRadius": 10,
    "measuringInterval": 60,
    "measuringDescription": "",
    "stationary": true,
    "dataLink": "https://wp.oulunliikenne.fi/avoin-data/pyorailykavely/graphql-rajapinnat/#py%C3%B6r%C3%A4ily-ja-k%C3%A4vely-m%C3%A4%C3%A4r%C3%A4t",
    "dataLatestValue": null
  },
  {
    "id": "OULU174_PP6_PP5",
    "crsType": "EPSG:4326",
    "iconName": "motion",
    "location": { "lat": 65.062701, "lng": 25.514529, "elevation": 0 },
    "status": "Online",
    "sensorType": "Counter",
    "description": "PP6_PP5",
    "IsDataSecret": false,
    "measuringDirection": [0, 90],
    "measuringRadius": 10,
    "measuringInterval": 60,
    "measuringDescription": "",
    "stationary": true,
    "dataLink": "https://wp.oulunliikenne.fi/avoin-data/pyorailykavely/graphql-rajapinnat/#py%C3%B6r%C3%A4ily-ja-k%C3%A4vely-m%C3%A4%C3%A4r%C3%A4t",
    "dataLatestValue": null
  },
  
  */