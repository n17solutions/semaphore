using System;
using N17Solutions.Semaphore.Domain.Model;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Domain.Tests.Model
{
    public class SignalTests
    {
        [Fact]
        public void Should_Map_Signal_To_SignalResponse()
        {
            // Arrange
            var domainModel = new Signal
            {
                Id = 1,
                ResourceId = Guid.NewGuid(),
                Name = "Test Signal",
                Value = "Test Value",
                Tags = "Test,Tags",
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
            
            // Act
            var result = SignalExpressions.ToSignalResponse.Compile()(domainModel);
            
            // Assert
            result.ResourceId.ShouldBe(domainModel.ResourceId);
            result.Name.ShouldBe(domainModel.Name);
            result.Value.ShouldBe(domainModel.Value);
        }
        
        [Fact]
        public void Should_Map_Signal_To_SignalResponse_When_ResourceId_Is_Null()
        {
            // Arrange
            var domainModel = new Signal
            {
                Id = 1,
                Name = "Test Signal",
                Value = "Test Value",
                Tags = "Test,Tags",
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
            
            // Act
            var result = SignalExpressions.ToSignalResponse.Compile()(domainModel);
            
            // Assert
            result.ResourceId.ShouldBe(domainModel.ResourceId);
            result.Name.ShouldBe(domainModel.Name);
            result.Value.ShouldBe(domainModel.Value);
        }
    }
}