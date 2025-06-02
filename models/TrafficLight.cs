public class TrafficLightRoot
{
    public TrafficLightResponse Data { get; set; } = new();
}

public class TrafficLightResponse
{
    public List<TrafficLight> TrafficLights { get; set; } = new();
}

public class TrafficLight
{
    public required string DevName { get; set; } = string.Empty;
    public required string MeasuredTime { get; set; }
    public required List<TrafficLightValues> Values { get; set; }
}
public class TrafficLightValues
{
    public string SgName { get; set; } = string.Empty;
    public required string DetName { get; set; }
    public required string Name { get; set; }
    public int Value { get; set; }
    public required string Unit { get; set; }
    public int Interval { get; set; }
    public int ReliabValue { get; set; }

}