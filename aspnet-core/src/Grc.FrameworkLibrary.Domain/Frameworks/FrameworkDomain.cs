using System;
using Grc.ValueObjects;
using Volo.Abp.Domain.Entities;

namespace Grc.FrameworkLibrary.Domain.Frameworks;

/// <summary>
/// Framework domain/category
/// </summary>
public class FrameworkDomain : Entity<Guid>
{
    public Guid FrameworkId { get; private set; }
    public string Code { get; private set; }
    public LocalizedString Name { get; private set; }
    public LocalizedString Description { get; private set; }
    public int SortOrder { get; private set; }
    public DateTime CreationTime { get; private set; }
    
    protected FrameworkDomain() { }
    
    public FrameworkDomain(Guid id, Guid frameworkId, string code, LocalizedString name)
    {
        Id = id;
        FrameworkId = frameworkId;
        Code = code;
        Name = name;
        CreationTime = DateTime.UtcNow;
    }
    
    public void SetDescription(LocalizedString description)
    {
        Description = description;
    }
    
    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
    }
}

