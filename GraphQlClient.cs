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

    public async Task<string> SendQueryAsync(string endpoint, string query, object? variables = null)
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


        return responseBody;
    }
}