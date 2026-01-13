using FluentAssertions;
using GrcMvc.Models.Entities;
using GrcMvc.Models.Interfaces;
using Xunit;

namespace GrcMvc.Tests.Models;

/// <summary>
/// Unit tests for BaseEntity to verify IGovernedResource implementation
/// </summary>
public class BaseEntityTests
{
    // Test entity class for testing BaseEntity
    private class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void BaseEntity_ImplementsIGovernedResource_PropertiesExist()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };

        // Act & Assert
        entity.Should().BeAssignableTo<IGovernedResource>();
        entity.Owner.Should().BeNull(); // Default value
        entity.DataClassification.Should().BeNull(); // Default value
        entity.Labels.Should().NotBeNull();
        entity.Labels.Should().BeEmpty(); // Default empty dictionary
        entity.ResourceType.Should().Be("TestEntity"); // Should return class name
    }

    [Fact]
    public void BaseEntity_LabelsSerialization_WorksCorrectly()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };
        var labels = new Dictionary<string, string>
        {
            { "dataClassification", "confidential" },
            { "owner", "IT-Security" },
            { "approvedForProd", "true" }
        };

        // Act
        entity.Labels = labels;

        // Assert
        entity.LabelsJson.Should().NotBeNull();
        entity.LabelsJson.Should().Contain("confidential");
        entity.LabelsJson.Should().Contain("IT-Security");
        entity.LabelsJson.Should().Contain("approvedForProd");
        
        // Verify round-trip serialization
        var labels2 = entity.Labels;
        labels2.Should().HaveCount(3);
        labels2["dataClassification"].Should().Be("confidential");
        labels2["owner"].Should().Be("IT-Security");
        labels2["approvedForProd"].Should().Be("true");
    }

    [Fact]
    public void BaseEntity_LabelsEmptyDictionary_SerializesToNull()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };
        entity.Labels = new Dictionary<string, string> { { "key", "value" } };

        // Act
        entity.Labels = new Dictionary<string, string>();

        // Assert
        entity.LabelsJson.Should().BeNull();
        entity.Labels.Should().NotBeNull();
        entity.Labels.Should().BeEmpty();
    }

    [Fact]
    public void BaseEntity_LabelsNull_HandlesGracefully()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };
        entity.Labels = new Dictionary<string, string> { { "key", "value" } };

        // Act
        entity.Labels = null!;

        // Assert
        entity.LabelsJson.Should().BeNull();
        entity.Labels.Should().NotBeNull();
        entity.Labels.Should().BeEmpty();
    }

    [Fact]
    public void BaseEntity_LabelsInvalidJson_ReturnsEmptyDictionary()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };
        entity.LabelsJson = "{ invalid json }";

        // Act
        var labels = entity.Labels;

        // Assert
        labels.Should().NotBeNull();
        labels.Should().BeEmpty(); // Should handle gracefully
    }

    [Fact]
    public void BaseEntity_OwnerProperty_CanBeSetAndRetrieved()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };

        // Act
        entity.Owner = "IT-Security-Team";

        // Assert
        entity.Owner.Should().Be("IT-Security-Team");
    }

    [Fact]
    public void BaseEntity_DataClassificationProperty_CanBeSetAndRetrieved()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };

        // Act
        entity.DataClassification = "restricted";

        // Assert
        entity.DataClassification.Should().Be("restricted");
    }

    [Fact]
    public void BaseEntity_ResourceType_ReturnsClassName()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };

        // Act
        var resourceType = entity.ResourceType;

        // Assert
        resourceType.Should().Be("TestEntity");
    }
}
