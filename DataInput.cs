using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class DataInput
{
    public static void InputDevices(IServiceProvider services, string jsonFilePath)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DeviceDb>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DataInput");

        if (context.Devices.Any())
        {
            logger.LogInformation("Devices already input, skipping.");
            return;
        }

        try
        {
            var json = File.ReadAllText(jsonFilePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var devices = JsonSerializer.Deserialize<List<Device>>(json, options);
            if (devices != null)
            {
                context.Devices.AddRange(devices);
                context.SaveChanges();
                logger.LogInformation("Input {Count} devices.", devices.Count);
            }
            else
            {
                logger.LogWarning("No devices found in JSON.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inputting devices from JSON");
        }
    }

    public static void InputDeviceList(IServiceProvider services, List<Device> deviceList)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DeviceDb>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DataInputList");

        try
        {
            if (deviceList != null && deviceList.Any())
            {
                context.Devices.AddRange(deviceList);
                context.SaveChanges();
                logger.LogInformation("Input {Count} devices.", deviceList.Count);
            }
            else
            {
                logger.LogWarning("No devices to input.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inputting devices.");
        }
    }
}
