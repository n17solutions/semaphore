using System;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.ServiceContract;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Domain.Tests.Model
{
    public class SignalTests
    {
        [Fact]
        public void Should_Map_String_Signal_To_SignalResponse()
        {
            // Arrange
            var domainModel = new Signal
            {
                Id = 1,
                ResourceId = Guid.NewGuid(),
                Name = "Test Signal",
                Value = "Test Value",
                ValueType = typeof(object).FullName,
                IsBaseType = false,
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
            result.ValueType.ShouldBe(domainModel.ValueType);
            result.IsBaseType.ShouldBe(domainModel.IsBaseType);
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
                ValueType = typeof(string).FullName,
                IsBaseType = true,
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
            result.ValueType.ShouldBe(domainModel.ValueType);
            result.IsBaseType.ShouldBe(domainModel.IsBaseType);
        }

        [Theory]
        [InlineData(Constants.EncryptedTag, true)]
        [InlineData("SomeTag", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Should_Map_IsEncrypted_Properly_Based_On_Encrypted_Tag(string tags, bool expected)
        {
            // Arrange
            var domainModel = new Signal
            {
                Id = 1,
                Name = "Test Signal",
                Value = "Test Value",
                ValueType = typeof(string).FullName,
                IsBaseType = true,
                Tags = tags,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
            
            // Act
            var result = SignalExpressions.ToSignalResponse.Compile()(domainModel);
            
            // Assert
            result.IsEncrypted.ShouldBe(expected);
        }
    }
}