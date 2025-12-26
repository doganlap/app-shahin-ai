using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Grc.Elasticsearch;

/// <summary>
/// Elasticsearch service for full-text search
/// </summary>
public class ElasticsearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchService> _logger;
    private const string IndexName = "grc-controls";

    public ElasticsearchService(IConfiguration configuration, ILogger<ElasticsearchService> logger)
    {
        var url = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
        var settings = new ElasticsearchClientSettings(new Uri(url));
        _client = new ElasticsearchClient(settings);
        _logger = logger;
    }

    /// <summary>
    /// Index a control for search
    /// </summary>
    public async Task IndexControlAsync(ControlDocument control)
    {
        try
        {
            await _client.IndexAsync(control, idx => idx.Index(IndexName));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing control {ControlId}", control.Id);
        }
    }

    /// <summary>
    /// Search controls
    /// </summary>
    public async Task<List<ControlDocument>> SearchControlsAsync(string query, List<Guid> frameworkIds = null)
    {
        var searchRequest = new SearchRequest(IndexName)
        {
            Query = new BoolQuery
            {
                Must = new Query[]
                {
                    new MultiMatchQuery
                    {
                        Query = query,
                        Fields = new[] { "title.en", "title.ar", "requirement.en", "requirement.ar", "controlNumber" },
                        Type = TextQueryType.BestFields
                    }
                }
            }
        };

        if (frameworkIds != null && frameworkIds.Count > 0)
        {
            searchRequest.Query = new BoolQuery
            {
                Must = new Query[]
                {
                    new MultiMatchQuery
                    {
                        Query = query,
                        Fields = new[] { "title.en", "title.ar", "requirement.en", "requirement.ar" }
                    },
                    new TermsQuery
                    {
                        Field = "frameworkId",
                        Terms = frameworkIds
                    }
                }
            };
        }

        var response = await _client.SearchAsync<ControlDocument>(searchRequest);
        
        if (response.IsValidResponse)
        {
            return response.Documents.ToList();
        }

        return new List<ControlDocument>();
    }

    /// <summary>
    /// Create index with Arabic analyzer
    /// </summary>
    public async Task CreateIndexAsync()
    {
        var indexSettings = new
        {
            settings = new
            {
                analysis = new
                {
                    analyzer = new
                    {
                        arabic_analyzer = new
                        {
                            type = "custom",
                            tokenizer = "standard",
                            filter = new[] { "lowercase", "arabic_normalization", "arabic_stemmer" }
                        }
                    }
                }
            },
            mappings = new
            {
                properties = new
                {
                    title = new { type = "object", properties = new { en = new { type = "text" }, ar = new { type = "text", analyzer = "arabic_analyzer" } } },
                    requirement = new { type = "object", properties = new { en = new { type = "text" }, ar = new { type = "text", analyzer = "arabic_analyzer" } } },
                    controlNumber = new { type = "keyword" },
                    frameworkId = new { type = "keyword" }
                }
            }
        };

        // TODO: Implement index creation
        await Task.CompletedTask;
    }
}

/// <summary>
/// Control document for Elasticsearch
/// </summary>
public class ControlDocument
{
    public Guid Id { get; set; }
    public Guid FrameworkId { get; set; }
    public string ControlNumber { get; set; }
    public LocalizedStringDocument Title { get; set; }
    public LocalizedStringDocument Requirement { get; set; }
}

/// <summary>
/// Localized string document
/// </summary>
public class LocalizedStringDocument
{
    public string En { get; set; }
    public string Ar { get; set; }
}

