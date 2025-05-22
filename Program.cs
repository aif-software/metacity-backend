using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DeviceDb>(opt => opt.UseInMemoryDatabase("devices"));
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
app.MapGet("/devices", async (DeviceDb db) =>
    await db.Devices.ToListAsync());

/** GET Device by Id */
app.MapGet("/devices/{id}", async (string id, DeviceDb db) =>
   await db.Devices.FindAsync(id)
    is Device Device
        ? Results.Ok(Device)
        : Results.NotFound());

/** GET Device by sensortype */
app.MapGet("/devices/sensorType", async (string inputSensorType, DeviceDb db) =>
  {
      var devices = await db.Devices
          .Where(d => d.sensorType == inputSensorType)
          .ToListAsync();

      return devices.Any()
          ? Results.Ok(devices)
          : Results.NotFound($"No devices found with sensorType '{inputSensorType}'");
  });

/** GET Device by status */
app.MapGet("/devices/status", async (string inputStatus, DeviceDb db) =>
  {
      var devices = await db.Devices
          .Where(d => d.status == inputStatus)
          .ToListAsync();

      return devices.Any()
          ? Results.Ok(devices)
          : Results.NotFound($"No devices found with status '{inputStatus}'");
  });

/** GET Device by elevation */
app.MapGet("/devices/elevation", async (double inputMinElevation, double inputMaxElevation, DeviceDb db) =>
  {
      var devices = await db.Devices
          .Where(d => d.location.elevation > inputMinElevation && d.location.elevation < inputMaxElevation)
          .ToListAsync();

      return devices.Any()
          ? Results.Ok(devices)
          : Results.NotFound($"No devices found with elevation between '{inputMinElevation}' and '{inputMaxElevation}'");
  });

/** GET all devices where data is secret */
app.MapGet("/devices/isdatasecret", async (DeviceDb db) =>
    await db.Devices.Where(d => d.isDataSecret).ToListAsync());

/** POST Device */
app.MapPost("/devices", async (Device Device, DeviceDb db) =>
{
    db.Devices.Add(Device);
    await db.SaveChangesAsync();

    return Results.Created($"/devices/{Device.id}", Device);
});

/** PUT Device */
app.MapPut("/devices/{id}", async (string id, Device inputDevice, DeviceDb db) =>
{
    var device = await db.Devices.FindAsync(id);

    if (device is null) return Results.NotFound();

    device.id = inputDevice.id;
    device.crsType = inputDevice.crsType;
    device.iconName = inputDevice.iconName;
    device.location = inputDevice.location;
    device.status = inputDevice.status;
    device.sensorType = inputDevice.sensorType;
    device.sensorModel = inputDevice.sensorModel;
    device.description = inputDevice.description;
    device.isDataSecret = inputDevice.isDataSecret;
    device.dataLink = inputDevice.dataLink;
    device.measuringDirection = inputDevice.measuringDirection;
    device.measuringRadius = inputDevice.measuringRadius;
    device.measuringInterval = inputDevice.measuringInterval;
    device.measuringDescription = inputDevice.measuringDescription;
    device.stationary = inputDevice.stationary;
    device.dataLatestValue = inputDevice.dataLatestValue;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

/** DELETE Device */
app.MapDelete("/devices/{id}", async (string id, DeviceDb db) =>
{
    if (await db.Devices.FindAsync(id) is Device Device)
    {
        db.Devices.Remove(Device);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

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
List<Device> result = await graphQLClient.SendQueryAsync(endpoint, query);

DataInput.InputDeviceList(app.Services, result);
//Console.WriteLine(result);

app.Run();