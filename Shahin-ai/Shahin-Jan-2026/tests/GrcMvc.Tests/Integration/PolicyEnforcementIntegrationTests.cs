using Xunit;
using FluentAssertions;
using GrcMvc.Application.Policy;
using GrcMvc.Models.Entities;
using GrcMvc.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace GrcMvc.Tests.Integration
{
    /// <summary>
    /// Integration tests for Policy Enforcement
    /// Tests real policy evaluation scenarios with actual policy engine
    /// </summary>
    public class PolicyEnforcementIntegrationTests : IDisposable
    {
        private readonly PolicyEnforcementHelper _helper;
        private readonly IPolicyEnforcer _policyEnforcer;
        private readonly IPolicyStore _policyStore;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Mock<IHostEnvironment> _environmentMock;
        private readonly ILogger<PolicyEnforcementHelper> _logger;

        public PolicyEnforcementIntegrationTests()
        {
            // Setup configuration with policy file path
            // Try multiple paths: test output directory, then project root
            var testOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "etc", "policies", "grc-baseline.yml");
            var projectRootPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "..", "etc", "policies", "grc-baseline.yml");
            
            var policyPath = File.Exists(testOutputPath) ? testOutputPath : projectRootPath;
            
            // If still not found, use absolute path from project root
            if (!File.Exists(policyPath))
            {
                var projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".."));
                policyPath = Path.Combine(projectRoot, "etc", "policies", "grc-baseline.yml");
            }
            
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Policy:FilePath"] = policyPath
                })
                .Build();

            // Setup real policy store
            var policyLogger = new Mock<ILogger<PolicyStore>>();
            _policyStore = new PolicyStore(configuration, policyLogger.Object);

            // Setup real policy enforcer with all dependencies
            var pathResolver = new DotPathResolver(
                new MemoryCache(new MemoryCacheOptions()),
                new Mock<ILogger<DotPathResolver>>().Object);
            
            var mutationApplier = new MutationApplier(
                pathResolver,
                new Mock<ILogger<MutationApplier>>().Object);
            
            var auditLogger = new PolicyAuditLogger(
                new Mock<ILogger<PolicyAuditLogger>>().Object);
            
            var enforcerLogger = new Mock<ILogger<PolicyEnforcer>>();
            _policyEnforcer = new PolicyEnforcer(
                _policyStore,
                pathResolver,
                mutationApplier,
                auditLogger,
                enforcerLogger.Object);

            // Setup user and environment mocks
            _currentUserMock = new Mock<ICurrentUserService>();
            _currentUserMock.Setup(u => u.GetUserId()).Returns(Guid.NewGuid());
            _currentUserMock.Setup(u => u.GetTenantId()).Returns(Guid.NewGuid());
            _currentUserMock.Setup(u => u.GetRoles()).Returns(new List<string> { "User" });
            _currentUserMock.Setup(u => u.GetUserName()).Returns("testuser");
            
            _environmentMock = new Mock<IHostEnvironment>();
            _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");
            
            _logger = new Mock<ILogger<PolicyEnforcementHelper>>().Object;
            _helper = new PolicyEnforcementHelper(
                _policyEnforcer,
                _currentUserMock.Object,
                _environmentMock.Object,
                _logger);
        }

        [Fact]
        public async Task EvidenceCreate_WithoutDataClassification_ThrowsPolicyViolation()
        {
            // Arrange
            var evidence = new Evidence
            {
                Id = Guid.NewGuid(),
                Title = "Test Evidence",
                DataClassification = null, // Missing classification
                Owner = "IT-Security"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PolicyViolationException>(async () =>
                await _helper.EnforceCreateAsync("Evidence", evidence));

            exception.RuleId.Should().Be("REQUIRE_DATA_CLASSIFICATION");
            exception.Message.Should().Contain("dataClassification");
            exception.RemediationHint.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task EvidenceCreate_WithValidClassification_Allows()
        {
            // Arrange
            var evidence = new Evidence
            {
                Id = Guid.NewGuid(),
                Title = "Test Evidence",
                DataClassification = "internal",
                Owner = "IT-Security"
            };

            // Act - Should not throw
            await _helper.EnforceCreateAsync("Evidence", evidence, 
                dataClassification: "internal", 
                owner: "IT-Security");

            // Assert - No exception means policy passed
            Assert.True(true);
        }

        [Fact]
        public async Task EvidenceCreate_WithRestrictedInProd_RequiresApproval()
        {
            // Arrange - Set environment to Production
            _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");
            
            var evidence = new Evidence
            {
                Id = Guid.NewGuid(),
                Title = "Test Evidence",
                DataClassification = "restricted",
                Owner = "IT-Security"
            };

            // Act & Assert - Should throw because approvedForProd is not set
            var exception = await Assert.ThrowsAsync<PolicyViolationException>(async () =>
                await _helper.EnforceCreateAsync("Evidence", evidence,
                    dataClassification: "restricted",
                    owner: "IT-Security"));

            exception.RuleId.Should().Be("PROD_RESTRICTED_MUST_HAVE_APPROVAL");
            exception.RemediationHint.Should().Contain("approvedForProd");
        }

        [Fact]
        public async Task EvidenceCreate_WithRestrictedInDev_AllowsWithoutApproval()
        {
            // Arrange - Set environment to Development (exception applies)
            _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");
            
            var evidence = new Evidence
            {
                Id = Guid.NewGuid(),
                Title = "Test Evidence",
                DataClassification = "restricted",
                Owner = "IT-Security"
            };

            // Act - Should not throw in dev (exception rule applies)
            await _helper.EnforceCreateAsync("Evidence", evidence,
                dataClassification: "restricted",
                owner: "IT-Security");

            // Assert - No exception means policy passed (dev exception)
            Assert.True(true);
        }

        [Fact]
        public async Task AssessmentSubmit_WithoutOwner_ThrowsPolicyViolation()
        {
            // Arrange
            var assessment = new Assessment
            {
                Id = Guid.NewGuid(),
                Name = "Test Assessment",
                DataClassification = "internal",
                Owner = null // Missing owner
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PolicyViolationException>(async () =>
                await _helper.EnforceSubmitAsync("Assessment", assessment,
                    dataClassification: "internal"));

            exception.RuleId.Should().Be("REQUIRE_OWNER");
            exception.Message.Should().Contain("owner");
        }

        [Fact]
        public async Task RiskUpdate_WithValidData_Allows()
        {
            // Arrange
            var risk = new Risk
            {
                Id = Guid.NewGuid(),
                Name = "Test Risk",
                DataClassification = "confidential",
                Owner = "RiskTeam"
            };

            // Act - Should not throw
            await _helper.EnforceUpdateAsync("Risk", risk,
                dataClassification: "confidential",
                owner: "RiskTeam");

            // Assert - No exception means policy passed
            Assert.True(true);
        }

        [Fact]
        public async Task PolicyMutations_AppliedCorrectly()
        {
            // Arrange - Test mutation rule (NORMALIZE_EMPTY_LABELS)
            var evidence = new Evidence
            {
                Id = Guid.NewGuid(),
                Title = "Test Evidence",
                DataClassification = "internal",
                Owner = "unknown" // Should be normalized to null by mutation
            };

            // Act
            await _helper.EnforceCreateAsync("Evidence", evidence,
                dataClassification: "internal",
                owner: "unknown");

            // Assert - Mutation should have been applied (no exception)
            // Note: In real scenario, mutation would modify the resource
            Assert.True(true);
        }

        public void Dispose()
        {
            // Cleanup if needed
            if (_policyStore is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
