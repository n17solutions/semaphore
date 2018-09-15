using System;
using System.Collections.Generic;
using N17Solutions.Semaphore.Domain.Model;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Domain.Tests.Model
{
    public class FeatureTests
    {
        [Fact]
        public void Should_Map_Feature_To_FeatureFlagResponse()
        {
            // Arrange
            var domainModel = new Feature
            {
                Id = 1,
                ResourceId = Guid.NewGuid(),
                Name = "Test Feature",
                Signals = new List<Signal>
                {
                    new Signal
                    {
                        Id = 1,
                        Value = "true",
                        Tags = "Test,Tags",
                        DateCreated = DateTime.Now,
                        DateLastUpdated = DateTime.Now
                    },
                    new Signal
                    {
                        Id = 2,
                        Value = "false",
                        Tags = "Test,Tags,Again",
                        DateCreated = DateTime.Now,
                        DateLastUpdated = DateTime.Now
                    }
                },
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
            
            // Act
            var result = FeatureExpressions.ToFeatureFlagResponse.Compile()(domainModel);
            
            // Assert
            result.ResourceId.ShouldBe(domainModel.ResourceId);
            result.Name.ShouldBe(domainModel.Name);
            result.Settings.Length.ShouldBe(domainModel.Signals.Count);
        }
    }
}