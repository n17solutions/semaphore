using System.Collections.Generic;
using N17Solutions.Semaphore.ServiceContract.Extensions;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.ServiceContract.Tests.Extensions
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void IsNullOrEmpty_Should_Return_True_When_Target_Is_Null()
        {
            // Act
            var result = ((IEnumerable<string>) null).IsNullOrEmpty();
            
            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_Should_Return_True_When_Target_Is_Empty()
        {
            // Arrange
            var target = new string[0];
            
            // Act
            var result = target.IsNullOrEmpty();
            
            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_Should_Return_False_When_Target_Has_Items()
        {
            // Arrange
            var target = new[] {"Item1", "Item2"};
            
            // Act
            var result = target.IsNullOrEmpty();
            
            // Assert
            result.ShouldBeFalse();
        }

    }
}