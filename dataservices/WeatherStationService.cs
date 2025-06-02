using System.Text.Json;

public static class WeatherStationService
{
    public static async Task<List<Device>> GetWeatherStationsAsync()
    {
        var query = @"
        query GetAllCityWeatherStations { 
          cityWeatherStations {
            weatherStationId
            name
            lat
            lon
            sensorValues {
              name
              sensorId
              sensorValue
              sensorUnit
              measuredTime
            }
            cameras {
              cameraId
              imageUrl
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

        var root = JsonSerializer.Deserialize<WeatherRoot>(result, options);

        var weatherStations = new List<Device>();

        if (root?.Data?.CityWeatherStations != null)
        {
            foreach (var station in root.Data.CityWeatherStations)
            {
                var deviceLocation = new Location
                {
                    lat = station.Lat,
                    lng = station.Lon,
                    elevation = 3.0
                };
                var deviceId = IdIndexService.GetId();

                var newDevice = new Device
                {
                    id = deviceId,
                    name = station.WeatherStationId,
                    crsType = "EPSG:4326",
                    iconName = "temperature",
                    location = deviceLocation,
                    status = "Online",
                    sensorType = "weather",
                    description = station.Name,
                    isDataSecret = false,
                    dataLink = station.Cameras[0].ImageUrl,
                    measuringDirection = [-180, 180],
                    measuringRadius = 10,
                    measuringInterval = 300,
                    stationary = true,
                    dataLatestValue = null,
                };
                if (station.SensorValues != null)
                {
                    foreach (var sensor in station.SensorValues)
                    {
                        WeatherData weather = MapSensorsToWeatherData(station.SensorValues);
                        newDevice.weatherData = weather;
                    }
                }

                weatherStations.Add(newDevice);
            }
        }

        return weatherStations;
    }

    public static WeatherData MapSensorsToWeatherData(List<Values> sensors)
    {
        var weatherData = new WeatherData();

        foreach (var sensor in sensors)
        {
            switch (sensor.Name)
            {
                case "AIR_TEMPERATURE":
                    weatherData.air_temperature = sensor.SensorValue;
                    break;
                case "AIR_RELATIVE_HUMIDITY":
                    weatherData.air_relative_humidity = sensor.SensorValue;
                    break;
                case "DEW_POINT_TEMPERATURE":
                    weatherData.dew_point_temperature = sensor.SensorValue;
                    break;
                case "WIND_SPEED":
                    weatherData.wind_speed = sensor.SensorValue;
                    break;
                case "WIND_DIRECTION":
                    weatherData.wind_direction = sensor.SensorValue;
                    break;
                case "RAINFALL_DEPTH":
                    weatherData.rainfall_depth = sensor.SensorValue;
                    break;
                case "RAINFALL_INTENSITY":
                    weatherData.rainfall_intensity = sensor.SensorValue;
                    break;
                case "SNOW_DEPTH_A":
                    weatherData.snow_depth_a = sensor.SensorValue;
                    break;
                case "SNOW_DEPTH_B":
                    weatherData.snow_depth_b = sensor.SensorValue;
                    break;
                case "SNOW_DEPTH_C":
                    weatherData.snow_depth_c = sensor.SensorValue;
                    break;
                case "SNOW_DEPTH":
                    weatherData.snow_depth = sensor.SensorValue;
                    break;
                case "RAIN_CLASSIFICATION":
                    weatherData.rain_classification = sensor.SensorValue;
                    break;
                case "ROAD_SURFACE_TEMPERATURE":
                    weatherData.road_surface_temperature = sensor.SensorValue;
                    break;
            }
        }

        return weatherData;
    }

}