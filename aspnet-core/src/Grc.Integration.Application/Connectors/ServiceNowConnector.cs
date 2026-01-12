using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Grc.Integration.Application.Connectors;

/// <summary>
/// ServiceNow connector
/// </summary>
public class ServiceNowConnector
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServiceNowConnector> _logger;
    private readonly string _baseUrl;
    private readonly string _username;
    private readonly string _password;

    public ServiceNowConnector(
        string baseUrl,
        string username,
        string password,
        ILogger<ServiceNowConnector> logger)
    {
        _baseUrl = baseUrl;
        _username = username;
        _password = password;
        _logger = logger;
        _httpClient = new HttpClient();
        
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
    }

    /// <summary>
    /// Create incident in ServiceNow
    /// </summary>
    public async Task<string> CreateIncidentAsync(string shortDescription, string description, string category)
    {
        var incident = new
        {
            short_description = shortDescription,
            description = description,
            category = category,
            urgency = "2",
            impact = "2"
        };

        var json = JsonSerializer.Serialize(incident);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/now/table/incident", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        return result.GetProperty("result").GetProperty("sys_id").GetString();
    }

    /// <summary>
    /// Sync compliance items from ServiceNow
    /// </summary>
    public async Task<List<ServiceNowComplianceItem>> SyncComplianceItemsAsync()
    {
        // TODO: Implement ServiceNow API calls to fetch compliance items
        return await Task.FromResult(new List<ServiceNowComplianceItem>());
    }
}

/// <summary>
/// ServiceNow compliance item
/// </summary>
public class ServiceNowComplianceItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string Category { get; set; }
}

