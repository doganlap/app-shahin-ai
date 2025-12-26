using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Grc.Integration.Application.Connectors;

/// <summary>
/// Jira connector
/// </summary>
public class JiraConnector
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JiraConnector> _logger;
    private readonly string _baseUrl;
    private readonly string _username;
    private readonly string _apiToken;

    public JiraConnector(
        string baseUrl,
        string username,
        string apiToken,
        ILogger<JiraConnector> logger)
    {
        _baseUrl = baseUrl;
        _username = username;
        _apiToken = apiToken;
        _logger = logger;
        _httpClient = new HttpClient();
        
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{apiToken}"));
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
    }

    /// <summary>
    /// Create issue in Jira
    /// </summary>
    public async Task<string> CreateIssueAsync(string projectKey, string summary, string description, string issueType = "Task")
    {
        var issue = new
        {
            fields = new
            {
                project = new { key = projectKey },
                summary = summary,
                description = description,
                issuetype = new { name = issueType }
            }
        };

        var json = JsonSerializer.Serialize(issue);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/rest/api/3/issue", content);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        return result.GetProperty("key").GetString();
    }

    /// <summary>
    /// Link control assessment to Jira issue
    /// </summary>
    public async Task LinkControlAssessmentAsync(Guid controlAssessmentId, string jiraIssueKey)
    {
        // TODO: Store mapping between control assessment and Jira issue
        await Task.CompletedTask;
    }
}

