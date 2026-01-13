using FluentAssertions;
using GrcMvc.Application.Policy;
using GrcMvc.Models.DTOs;
using GrcMvc.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GrcMvc.Tests.Controllers;

/// <summary>
/// Unit tests for policy enforcement helper behavior
/// Tests the PolicyEnforcementHelper directly with mocked dependencies
/// </summary>
public class PolicyEnforcementTests
{
    private readonly PolicyEnforcementHelper _policyHelper;
    private readonly Mock<IPolicyEnforcer> _mockPolicyEnforcer;
    private readonly Mock<ICurrentUserService> _mockCurrentUser;
    private readonly Mock<IHostEnvironment> _mockEnvironment;

    public PolicyEnforcementTests()
    {
        _mockPolicyEnforcer = new Mock<IPolicyEnforcer>();
        _mockCurrentUser = new Mock<ICurrentUserService>();
        _mockEnvironment = new Mock<IHostEnvironment>();
        var mockLogger = new Mock<ILogger<PolicyEnforcementHelper>>();

        // Setup default mocks
        _mockCurrentUser.Setup(u => u.GetUserId()).Returns(Guid.NewGuid());
        _mockCurrentUser.Setup(u => u.GetTenantId()).Returns(Guid.NewGuid());
        _mockCurrentUser.Setup(u => u.GetRoles()).Returns(new List<string> { "User" });
        _mockCurrentUser.Setup(u => u.GetUserName()).Returns("testuser");
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");

        _policyHelper = new PolicyEnforcementHelper(
            _mockPolicyEnforcer.Object,
            _mockCurrentUser.Object,
            _mockEnvironment.Object,
            mockLogger.Object);
    }

    [Fact]
    public async Task Controller_Create_WithValidData_PolicyEnforced()
    {
        // Arrange
        var dto = new CreateEvidenceDto
        {
            Name = "Test Evidence",
            DataClassification = "internal",
            Owner = "IT-Security"
        };

        _mockPolicyEnforcer
            .Setup(x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => 
                    ctx.Action == "create" && 
                    ctx.ResourceType == "Evidence"),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _policyHelper.EnforceCreateAsync(
            "Evidence",
            dto,
            dataClassification: dto.DataClassification,
            owner: dto.Owner);

        // Assert
        _mockPolicyEnforcer.Verify(
            x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => 
                    ctx.Action == "create" && 
                    ctx.ResourceType == "Evidence"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Controller_Create_WithMissingClassification_ThrowsPolicyViolation()
    {
        // Arrange
        var dto = new CreateEvidenceDto
        {
            Name = "Test Evidence",
            DataClassification = null, // Missing classification
            Owner = "IT-Security"
        };

        _mockPolicyEnforcer
            .Setup(x => x.EnforceAsync(
                It.IsAny<PolicyContext>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PolicyViolationException(
                "Missing/invalid metadata.labels.dataClassification",
                "REQUIRE_DATA_CLASSIFICATION",
                "Set metadata.labels.dataClassification to one of the allowed values."));

        // Act & Assert
        await Assert.ThrowsAsync<PolicyViolationException>(async () =>
            await _policyHelper.EnforceCreateAsync(
                "Evidence",
                dto,
                dataClassification: dto.DataClassification,
                owner: dto.Owner));
    }

    [Fact]
    public async Task Controller_Create_WithRestrictedInProd_RequiresApproval()
    {
        // Arrange
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
        var dto = new CreateEvidenceDto
        {
            Name = "Test Evidence",
            DataClassification = "restricted",
            Owner = "IT-Security"
        };

        _mockPolicyEnforcer
            .Setup(x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => 
                    ctx.Environment == "prod" && 
                    ctx.ResourceType == "Evidence"),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PolicyViolationException(
                "Restricted data in prod requires metadata.labels.approvedForProd=true",
                "PROD_RESTRICTED_MUST_HAVE_APPROVAL",
                "Run the approval workflow and set approvedForProd=true."));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PolicyViolationException>(async () =>
            await _policyHelper.EnforceCreateAsync(
                "Evidence",
                dto,
                dataClassification: dto.DataClassification,
                owner: dto.Owner));

        exception.RuleId.Should().Be("PROD_RESTRICTED_MUST_HAVE_APPROVAL");
        exception.RemediationHint.Should().Contain("approvedForProd=true");
    }

    [Fact]
    public async Task Controller_Update_WithValidData_PolicyEnforced()
    {
        // Arrange
        var dto = new UpdateEvidenceDto
        {
            Name = "Updated Evidence",
            DataClassification = "confidential",
            Owner = "Compliance-Team"
        };

        _mockPolicyEnforcer
            .Setup(x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => ctx.Action == "update"),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _policyHelper.EnforceUpdateAsync(
            "Evidence",
            dto,
            dataClassification: dto.DataClassification,
            owner: dto.Owner);

        // Assert
        _mockPolicyEnforcer.Verify(
            x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => ctx.Action == "update"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Controller_Submit_WithValidData_PolicyEnforced()
    {
        // Arrange
        var dto = new CreateAssessmentDto
        {
            Name = "Test Assessment",
            DataClassification = "internal",
            Owner = "Compliance-Manager"
        };

        _mockPolicyEnforcer
            .Setup(x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => ctx.Action == "submit"),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _policyHelper.EnforceSubmitAsync(
            "Assessment",
            dto,
            dataClassification: dto.DataClassification,
            owner: dto.Owner);

        // Assert
        _mockPolicyEnforcer.Verify(
            x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => ctx.Action == "submit"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Controller_Approve_WithValidData_PolicyEnforced()
    {
        // Arrange
        var dto = new PolicyDto
        {
            Id = Guid.NewGuid(),
            Title = "Test Policy",
            DataClassification = "confidential",
            Owner = "Policy-Owner"
        };

        _mockPolicyEnforcer
            .Setup(x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => ctx.Action == "approve"),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _policyHelper.EnforceApproveAsync(
            "PolicyDocument",
            dto,
            dataClassification: dto.DataClassification,
            owner: dto.Owner);

        // Assert
        _mockPolicyEnforcer.Verify(
            x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => ctx.Action == "approve"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Controller_Delete_WithValidData_PolicyEnforced()
    {
        // Arrange
        var evidence = new EvidenceDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Evidence",
            DataClassification = "internal",
            Owner = "IT-Security"
        };

        _mockPolicyEnforcer
            .Setup(x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => ctx.Action == "delete"),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _policyHelper.EnforceAsync(
            "delete",
            "Evidence",
            evidence,
            dataClassification: evidence.DataClassification,
            owner: evidence.Owner);

        // Assert
        _mockPolicyEnforcer.Verify(
            x => x.EnforceAsync(
                It.Is<PolicyContext>(ctx => ctx.Action == "delete"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
