using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Grc.Domain.EventSourcing;

/// <summary>
/// Repository interface for event store
/// </summary>
public interface IEventStoreRepository : IRepository<StoredEvent, System.Guid>
{
    Task<List<StoredEvent>> GetEventsByAggregateAsync(string aggregateId);
    Task<List<StoredEvent>> GetEventsByTypeAsync(string eventType);
    Task<List<StoredEvent>> GetEventsByTimeRangeAsync(System.DateTime start, System.DateTime end);
}

