public class Channel
{
    public required string Id { get; set; }
    public required string SiteId { get; set; }
    public required string Name { get; set; }
    public required string Domain { get; set; }
    public required int UserType { get; set; }
    public required string Timezone { get; set; }
    public required int Interval { get; set; }
    public required int Sens { get; set; }
    public required double Lat { get; set; }
    public required double Lon { get; set; }
}

public class Site
{
    public required string Id { get; set; }
    public required string SiteId { get; set; }
    public required string Name { get; set; }
    public required string Domain { get; set; }
    public required int UserType { get; set; }
    public required string Timezone { get; set; }
    public required int Interval { get; set; }
    public required int Sens { get; set; }
    public required List<Channel> Channels { get; set; }
}

public class EcoCounterSitesData
{
    public required List<Site> EcoCounterSites { get; set; }
}

public class GraphQLResponse<T>
{
    public required T Data { get; set; }
}
