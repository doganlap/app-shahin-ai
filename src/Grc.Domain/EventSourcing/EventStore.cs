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
        var storedEvent = new StoredEvent
        {
            Id = Guid.NewGuid(),
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
        return await _repository.GetListAsync(e => 
            e.AggregateId == aggregateId && 
            e.Version >= fromVersion,
            orderBy: e => e.Version);
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
        return await _repository.GetListAsync(e => 
            e.AggregateId == aggregateId && 
            e.Timestamp <= until,
            orderBy: e => e.Version);
    }

    private async Task<int> GetNextVersionAsync(string aggregateId)
    {
        var lastEvent = await _repository.FindAsync(e => e.AggregateId == aggregateId, 
            orderBy: e => e.Version, 
            descending: true);
        
        return lastEvent?.Version + 1 ?? 1;
    }
}

/// <summary>
/// Stored event entity
/// </summary>
public class StoredEvent : Entity<Guid>
{
    public string AggregateId { get; set; }
    public string AggregateType { get; set; }
    public string EventType { get; set; }
    public string EventData { get; set; }
    public DateTime Timestamp { get; set; }
    public int Version { get; set; }
    public string UserId { get; set; }
    public string TenantId { get; set; }
}

