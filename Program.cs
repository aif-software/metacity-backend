using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DeviceDb>(opt => opt.UseInMemoryDatabase("Devices"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "MetacitySensorAPI";
    config.Title = "MetacitySensorAPI v1";
    config.Version = "v1";
});

// Allow CORS configuration
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
});

var app = builder.Build();

// Use CORS Configuration
app.UseCors(MyAllowSpecificOrigins);

// Input existing device list, placeholder for development
DataInput.InputDevices(app.Services, "devices.json");

// Swagger setup
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "MetacitySensorAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

/** GET Devices */
app.MapGet("/Devices", async (DeviceDb db) =>
    await db.Devices.ToListAsync());

/** GET Device by Id */
app.MapGet("/Devices/{id}", async (string Id, DeviceDb db) =>
   await db.Devices.FindAsync(Id)
    is Device Device
        ? Results.Ok(Device)
        : Results.NotFound());

/** GET Device by sensortype */
app.MapGet("/Devices/sensorType", async (string inputSensorType, DeviceDb db) =>
  {
      var devices = await db.Devices
          .Where(d => d.SensorType == inputSensorType)
          .ToListAsync();

      return devices.Any()
          ? Results.Ok(devices)
          : Results.NotFound($"No devices found with sensorType '{inputSensorType}'");
  });

/** GET Device by status */
app.MapGet("/Devices/status", async (string inputStatus, DeviceDb db) =>
  {
      var devices = await db.Devices
          .Where(d => d.Status == inputStatus)
          .ToListAsync();

      return devices.Any()
          ? Results.Ok(devices)
          : Results.NotFound($"No devices found with status '{inputStatus}'");
  });

/** GET Device by elevation */
app.MapGet("/Devices/elevation", async (double inputMinElevation, double inputMaxElevation, DeviceDb db) =>
  {
      var devices = await db.Devices
          .Where(d => d.Location.Elevation > inputMinElevation && d.Location.Elevation < inputMaxElevation)
          .ToListAsync();

      return devices.Any()
          ? Results.Ok(devices)
          : Results.NotFound($"No devices found with elevation between '{inputMinElevation}' and '{inputMaxElevation}'");
  });

/** GET all devices where data is secret */
app.MapGet("/Devices/IsDataSecret", async (DeviceDb db) =>
    await db.Devices.Where(d => d.IsDataSecret).ToListAsync());

/** POST Device */
app.MapPost("/Devices", async (Device Device, DeviceDb db) =>
{
    db.Devices.Add(Device);
    await db.SaveChangesAsync();

    return Results.Created($"/Devices/{Device.Id}", Device);
});

/** PUT Device */
app.MapPut("/Devices/{id}", async (string Id, Device inputDevice, DeviceDb db) =>
{
    var device = await db.Devices.FindAsync(Id);

    if (device is null) return Results.NotFound();

    device.Location = inputDevice.Location;
    device.CrsType = inputDevice.CrsType;
    device.Status = inputDevice.Status;
    device.SensorType = inputDevice.SensorType;
    device.Description = inputDevice.Description;
    device.IsDataSecret = inputDevice.IsDataSecret;
    device.DataLink = inputDevice.DataLink;
    device.measuringDirection = inputDevice.measuringDirection;
    device.dataLatestValue = inputDevice.dataLatestValue;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

/** DELETE Device */
app.MapDelete("/Devices/{id}", async (string Id, DeviceDb db) =>
{
    if (await db.Devices.FindAsync(Id) is Device Device)
    {
        db.Devices.Remove(Device);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();