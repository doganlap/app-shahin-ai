using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.ActionPlan;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.ActionPlan.Application;

/// <summary>
/// Application service for Action Plan operations
/// </summary>
[Authorize(GrcPermissions.Risks.Default)]
public class ActionPlanAppService : ApplicationService
{
    private readonly IRepository<ActionPlan, Guid> _actionPlanRepository;
    private readonly IRepository<ActionItem, Guid> _actionItemRepository;

    public ActionPlanAppService(
        IRepository<ActionPlan, Guid> actionPlanRepository,
        IRepository<ActionItem, Guid> actionItemRepository)
    {
        _actionPlanRepository = actionPlanRepository;
        _actionItemRepository = actionItemRepository;
    }

    public async Task<ActionPlan> CreateAsync(string name, Guid ownerUserId, DateTime startDate, DateTime targetEndDate)
    {
        var actionPlan = new ActionPlan(
            GuidGenerator.Create(),
            name,
            ownerUserId,
            startDate,
            targetEndDate);
        
        await _actionPlanRepository.InsertAsync(actionPlan);
        return actionPlan;
    }

    public async Task<ActionItem> AddActionItemAsync(
        Guid actionPlanId,
        string title,
        string description,
        Guid assignedToUserId,
        DateTime? dueDate = null)
    {
        var actionPlan = await _actionPlanRepository.GetAsync(actionPlanId);
        var item = actionPlan.AddActionItem(title, description, assignedToUserId, dueDate);
        
        await _actionPlanRepository.UpdateAsync(actionPlan);
        return item;
    }

    public async Task<List<ActionItem>> GetMyActionItemsAsync(Guid userId)
    {
        var items = await _actionItemRepository.GetListAsync(i => i.AssignedToUserId == userId);
        return items.OrderBy(i => i.DueDate ?? DateTime.MaxValue).ToList();
    }

    public async Task CompleteActionItemAsync(Guid itemId, string completionNotes = null)
    {
        var item = await _actionItemRepository.GetAsync(itemId);
        item.Complete(completionNotes);
        await _actionItemRepository.UpdateAsync(item);
    }
}

