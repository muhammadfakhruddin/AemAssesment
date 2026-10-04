using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace AemAssesment.Services
{
    public sealed class PlatformWellApiClient(HttpClient http, IConfiguration config)
    {
        // Case-insensitive so key casing in the response doesn't matter.
        private static readonly JsonNodeOptions CaseInsensitive = new() { PropertyNameCaseInsensitive = true };

        public async Task<JsonNode?> GetPlatformWells(CancellationToken ct)
        {
            var token = await Login(ct);
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = await http.GetStringAsync($"api/PlatformWell/GetPlatformWellActual", ct);
            return JsonNode.Parse(json, CaseInsensitive);
        }

        private async Task<string> Login(CancellationToken ct)
        {
            var response = await http.PostAsJsonAsync("api/Account/Login", new
            {
                username = config["ExternalApi:Username"],
                password = config["ExternalApi:Password"]
            }, ct);
            response.EnsureSuccessStatusCode();

            var node = JsonNode.Parse(await response.Content.ReadAsStringAsync(ct), CaseInsensitive);

            return (node is JsonObject obj ? obj["token"] : node)?.ToString() ?? throw new InvalidOperationException("Token not found in login response.");
        }
    }
}