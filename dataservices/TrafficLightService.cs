using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class TrafficLightService
{
    public static async Task<List<Device>> GetTrafficLightsAsync()
    {
        List<string> trafficLights = [
        "HA01LV",
        "OULU152",
        "OULU153",
        "OULU154",
        "OULU165",
        "OULU166",
        "OULU167",
        "OULU168",
        "OULU175",
        "OULU176"
        ];
        var url = "https://api.oulunliikenne.fi/tpm/kpi/traffic-volume/";

        var client = new HttpClient();


        foreach (var trafficLight in trafficLights)
        {
            var response = await client.GetAsync(url + trafficLight);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var parsed = JsonSerializer.Deserialize<TrafficLight>(jsonString, options);

                Console.WriteLine(parsed.DevName);
            }
            else
            {
                Console.WriteLine($"HTTP Request failed: {response.StatusCode}");
            }
        }

        var counterDevices = new List<Device>();

        return counterDevices;
    }
}
