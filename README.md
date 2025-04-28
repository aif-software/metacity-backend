# metacity-backend

This project was created using ASP.NET Core 9.0. It is used as a backend for the Metacity Device Map.

## Development server

Run `dotnet run` to host the backend. Navigate to `http://localhost:5120/swagger/` to see more documentation about the backend. To call the Devices endpoint, post a GET request to `http://localhost:5120/Devices/`.
<br />
To use the map that this backend serves, go to the repository of the [frontend](https://github.com/aif-software/metacity-sensor-map) and follow the directions there.

## Syntax for sensors

The sensors have the following syntax:

```
{
    "id": Name of the sensor, has to be unique,
    "crsType": Which CRS (Coordinate Reference System) the sensors coordinates use (Currently supported CRS are: EPSG:4326 and EPSG:3067). Most systems use EPSG:4326, but some finnish government agencies use EPSG:3067,
    "iconName": Name of the icon that is used to display inside marker.,
    "location": {
      "lat": Latitude coordinate,
      "lng": Longitude coordinate,
      "elevation": Elevation from ground in meters,
      "path": If sensor moves along a certain path, the coordinate points of that path in a array in format [{"lat": 12.345, "lng": 67.890}],
      "area": If sensor operates at a certain area, the coordinate points of that area in a array in format [{"lat": 12.345, "lng": 67.890}],
    },
    "status": Current status of sensor, can be Online, Offline or Maintenance,
    "sensorType": Type of sensor (Temperature, motion, drone, etc),
    "description": Short description of the location and/or the measurement area of the sensor,
    "IsDataSecret": Is the data the sensor provides secret in terms of GDPR, either true or false,
    "measuringDirection": Direction of measurement in degrees, with 0 being north, [-90, 90] being 180 degrees to north, and [-180, 180] being true circle. [-270, -90] is 180 degrees to south.,
    "measuringRadius": Radius of the sensors measurement area in meters.,
    "measuringInterval": Interval of the measurements of the sensors in seconds.,
    "measuringDescription": Short description of the measurement area, method or other notable things.,
    "stationary": Whether the sensor is stationary or not, either true or false.,
    "dataLink": Link to data if it is available. Preferably link straight to that particular sensors data, but could be a link to API,
    "dataLatestValue": Latest value of data produced by sensor if available.,
  }
```
