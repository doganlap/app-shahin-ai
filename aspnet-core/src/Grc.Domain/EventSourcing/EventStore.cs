using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Grc.Domain.EventSourcing;

/// <summary>
/// Event store for audit trail and event sourcing
/// </summary>
public class EventStore
{
    private readonly IEventStoreRepository _repository;

    public EventStore(IEventStoreRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Append event to store
    /// </summary>
    public async Task AppendEventAsync(
        string aggregateId,
        string aggregateType,
        string eventType,
        object eventData,
        int expectedVersion = -1)
    {
        var storedEvent = new StoredEvent(Guid.NewGuid())
        {
            AggregateId = aggregateId,
            AggregateType = aggregateType,
            EventType = eventType,
            EventData = System.Text.Json.JsonSerializer.Serialize(eventData),
            Timestamp = DateTime.UtcNow,
            Version = await GetNextVersionAsync(aggregateId)
        };

        // Optimistic concurrency check
        if (expectedVersion >= 0 && storedEvent.Version != expectedVersion + 1)
        {
            throw new InvalidOperationException($"Concurrency conflict: expected version {expectedVersion + 1}, got {storedEvent.Version}");
        }

        await _repository.InsertAsync(storedEvent);
    }

    /// <summary>
    /// Get all events for an aggregate
    /// </summary>
    public async Task<List<StoredEvent>> GetEventsAsync(string aggregateId, int fromVersion = 0)
    {
        var queryable = await _repository.GetQueryableAsync();
        var events = queryable
            .Where(e => e.AggregateId == aggregateId && e.Version >= fromVersion)
            .OrderBy(e => e.Version)
            .ToList();
        return events;
    }

    /// <summary>
    /// Replay events to reconstruct aggregate state
    /// </summary>
    public async Task<T> ReplayEventsAsync<T>(string aggregateId, T initialState, Func<T, StoredEvent, T> applyEvent)
    {
        var events = await GetEventsAsync(aggregateId);
        var state = initialState;

        foreach (var evt in events)
        {
            state = applyEvent(state, evt);
        }

        return state;
    }

    /// <summary>
    /// Get events for point-in-time reconstruction
    /// </summary>
    public async Task<List<StoredEvent>> GetEventsUntilAsync(string aggregateId, DateTime until)
    {
        var queryable = await _repository.GetQueryableAsync();
        var events = queryable
            .Where(e => e.AggregateId == aggregateId && e.Timestamp <= until)
            .OrderBy(e => e.Version)
            .ToList();
        return events;
    }

    private async Task<int> GetNextVersionAsync(string aggregateId)
    {
        var queryable = await _repository.GetQueryableAsync();
        var lastEvent = queryable
            .Where(e => e.AggregateId == aggregateId)
            .OrderByDescending(e => e.Version)
            .FirstOrDefault();
        
        return lastEvent?.Version + 1 ?? 1;
    }
}

/// <summary>
/// Stored event entity
/// </summary>
public class StoredEvent : Entity<Guid>
{
    public string AggregateId { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int Version { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    
    protected StoredEvent() { }
    
    public StoredEvent(Guid id) : base(id) { }
}

