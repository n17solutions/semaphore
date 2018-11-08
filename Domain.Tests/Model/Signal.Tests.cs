using System;
using System.Collections.Generic;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.ServiceContract;
using N17Solutions.Semaphore.ServiceContract.Signals;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        [Fact]
        public void Should_Map_String_Value_To_Write_Model()
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
            var result = domainModel.ToWriteModel();
            
            // Assert
            result.Name.ShouldBe(domainModel.Name);
            result.Tags.ShouldBe(domainModel.Tags.Split(','));
            result.Encrypted.ShouldBeFalse();
            result.Value.ShouldBe(domainModel.Value);
        }

        [Fact]
        public void Should_Map_Object_Value_To_WriteModel()
        {
            // Arrange
            const string name = "Test Name";
            
            var domainModel = new Signal
            {
                Id = 1,
                Name = "Test Signal",
                Value = JsonConvert.SerializeObject(new SignalWriteModel { Name = name }),
                ValueType = typeof(SignalWriteModel).AssemblyQualifiedName,
                IsBaseType = false,
                Tags = "Test,Tags",
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
            
            // Act
            var result = domainModel.ToWriteModel();
            
            // Assert
            result.Name.ShouldBe(domainModel.Name);
            result.Tags.ShouldBe(domainModel.Tags.Split(','));
            result.Encrypted.ShouldBeFalse();
            result.Value.ShouldBeOfType<SignalWriteModel>();
            ((SignalWriteModel)result.Value).Name.ShouldBe(name);
        }

        [Fact]
        public void Should_Populate_From_Object_WriteModel()
        {
            // Arrange
            var writeModel = new SignalWriteModel
            {
                Name = "Test Name",
                Tags = new List<string>{"Test", "Tags"},
                Encrypted = false,
                Value = new SignalWriteModel { Name = "Child" }
            };
            var domainModel = new Signal();
            
            // Act
            domainModel.PopulateFromWriteModel(writeModel);
            
            // Assert
            domainModel.Name.ShouldBe(writeModel.Name);
            domainModel.Tags.ShouldBe(string.Join(",", writeModel.Tags));
            domainModel.Value.ShouldBe(JsonConvert.SerializeObject(writeModel.Value));
        }

        [Fact]
        public void Should_Populate_From_Base_WriteModel()
        {
            // Arrange
            var writeModel = new SignalWriteModel
            {
                Name = "Test Name",
                Tags = new List<string>{"Test", "Tags"},
                Encrypted = false,
                Value = "Test Value"
            };
            var domainModel = new Signal();
            
            // Act
            domainModel.PopulateFromWriteModel(writeModel);
            
            // Assert
            domainModel.Name.ShouldBe(writeModel.Name);
            domainModel.Tags.ShouldBe(string.Join(",", writeModel.Tags));
            domainModel.Value.ShouldBe(writeModel.Value);
        }

        [Theory]
        [InlineData(Constants.EncryptedTag, true)]
        [InlineData("Tag", false)]
        [InlineData(null, false)]
        public void Should_Denote_If_IsEncrypted(string tag, bool expected)
        {
            // Arrange
            var domainModel = new Signal
            {
                Id = 1,
                Name = "Test Signal",
                Value = "Test",
                ValueType = typeof(string).FullName,
                IsBaseType = true,
                Tags = tag,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            };
            
            // Act
            var result = domainModel.IsEncrypted();
            
            // Assert
            result.ShouldBe(expected);
        }
    }
}