using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using GrcMvc.Data;
using GrcMvc.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.TenantManagement;
using Volo.Abp.MultiTenancy;

namespace GrcMvc.Tests.Integration;

/// <summary>
/// Integration tests for Trial Registration
/// Tests the actual service layer with in-memory database
/// </summary>
public class TrialRegistrationIntegrationTests : IDisposable
{
    private readonly GrcDbContext _dbContext;
    private readonly Mock<ILogger<GrcDbContext>> _logger;

    public TrialRegistrationIntegrationTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<GrcDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _logger = new Mock<ILogger<GrcDbContext>>();
        _dbContext = new GrcDbContext(options);

        // Seed test data if needed
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add any required seed data here
        // For example, default roles, settings, etc.
    }

    [Fact]
    public async Task CreateTenant_ShouldSaveToDatabase()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = tenantId,
            TenantSlug = $"test-tenant-{Guid.NewGuid():N}",
            OrganizationName = "Test Organization",
            AdminEmail = "admin@test.com",
            Email = "admin@test.com",
            Status = "Active",
            IsActive = true,
            IsTrial = true,
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionTier = "Trial",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            OnboardingStatus = "PENDING"
        };

        // Act
        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();

        // Assert
        var savedTenant = await _dbContext.Tenants.FindAsync(tenantId);
        savedTenant.Should().NotBeNull();
        savedTenant!.TenantSlug.Should().Be(tenant.TenantSlug);
        savedTenant.OrganizationName.Should().Be("Test Organization");
        savedTenant.IsTrial.Should().BeTrue();
    }

    [Fact]
    public async Task CreateTenant_WithDuplicateSlug_ShouldFail()
    {
        // Arrange
        var slug = $"duplicate-slug-{Guid.NewGuid():N}";
        var tenant1 = new Tenant
        {
            Id = Guid.NewGuid(),
            TenantSlug = slug,
            OrganizationName = "First Org",
            AdminEmail = "first@test.com",
            Email = "first@test.com",
            Status = "Active",
            IsActive = true,
            IsTrial = true,
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionTier = "Trial",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            OnboardingStatus = "PENDING"
        };

        var tenant2 = new Tenant
        {
            Id = Guid.NewGuid(),
            TenantSlug = slug, // Same slug
            OrganizationName = "Second Org",
            AdminEmail = "second@test.com",
            Email = "second@test.com",
            Status = "Active",
            IsActive = true,
            IsTrial = true,
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionTier = "Trial",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            OnboardingStatus = "PENDING"
        };

        // Act
        _dbContext.Tenants.Add(tenant1);
        await _dbContext.SaveChangesAsync();

        _dbContext.Tenants.Add(tenant2);
        
        // Assert - Should throw exception due to unique constraint
        var act = async () => await _dbContext.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task CreateTenantUser_ShouldLinkUserToTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        
        var tenant = new Tenant
        {
            Id = tenantId,
            TenantSlug = $"test-tenant-{Guid.NewGuid():N}",
            OrganizationName = "Test Organization",
            AdminEmail = "admin@test.com",
            Email = "admin@test.com",
            Status = "Active",
            IsActive = true,
            IsTrial = true,
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionTier = "Trial",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            OnboardingStatus = "PENDING"
        };

        var tenantUser = new TenantUser
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            RoleCode = "TenantAdmin",
            Status = "Active",
            ActivatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _dbContext.Tenants.Add(tenant);
        _dbContext.TenantUsers.Add(tenantUser);
        await _dbContext.SaveChangesAsync();

        // Assert
        var savedTenantUser = await _dbContext.TenantUsers
            .FirstOrDefaultAsync(tu => tu.TenantId == tenantId && tu.UserId == userId);
        
        savedTenantUser.Should().NotBeNull();
        savedTenantUser!.RoleCode.Should().Be("TenantAdmin");
        savedTenantUser.Status.Should().Be("Active");
    }

    [Fact]
    public async Task CreateOnboardingWizard_ShouldInitializeForTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = tenantId,
            TenantSlug = $"test-tenant-{Guid.NewGuid():N}",
            OrganizationName = "Test Organization",
            AdminEmail = "admin@test.com",
            Email = "admin@test.com",
            Status = "Active",
            IsActive = true,
            IsTrial = true,
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionTier = "Trial",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            OnboardingStatus = "PENDING"
        };

        var wizard = new OnboardingWizard
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CurrentStep = 1,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _dbContext.Tenants.Add(tenant);
        _dbContext.OnboardingWizards.Add(wizard);
        await _dbContext.SaveChangesAsync();

        // Assert
        var savedWizard = await _dbContext.OnboardingWizards
            .FirstOrDefaultAsync(w => w.TenantId == tenantId);
        
        savedWizard.Should().NotBeNull();
        savedWizard!.CurrentStep.Should().Be(1);
        savedWizard.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task QueryTenants_ShouldFilterByIsTrial()
    {
        // Arrange
        var trialTenant = new Tenant
        {
            Id = Guid.NewGuid(),
            TenantSlug = $"trial-{Guid.NewGuid():N}",
            OrganizationName = "Trial Org",
            AdminEmail = "trial@test.com",
            Email = "trial@test.com",
            Status = "Active",
            IsActive = true,
            IsTrial = true,
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionTier = "Trial",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            OnboardingStatus = "PENDING"
        };

        var paidTenant = new Tenant
        {
            Id = Guid.NewGuid(),
            TenantSlug = $"paid-{Guid.NewGuid():N}",
            OrganizationName = "Paid Org",
            AdminEmail = "paid@test.com",
            Email = "paid@test.com",
            Status = "Active",
            IsActive = true,
            IsTrial = false,
            SubscriptionStartDate = DateTime.UtcNow,
            SubscriptionTier = "Professional",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            OnboardingStatus = "COMPLETED"
        };

        _dbContext.Tenants.Add(trialTenant);
        _dbContext.Tenants.Add(paidTenant);
        await _dbContext.SaveChangesAsync();

        // Act
        var trialTenants = await _dbContext.Tenants
            .Where(t => t.IsTrial == true)
            .ToListAsync();

        // Assert
        trialTenants.Should().HaveCount(1);
        trialTenants.First().OrganizationName.Should().Be("Trial Org");
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}
