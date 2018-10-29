using N17Solutions.Semaphore.ServiceContract.Extensions;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.ServiceContract.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("  ", true)]
        [InlineData(" Data ", false)]
        [InlineData("Data", false)]
        public void Should_Output_Whether_String_Is_Null_Or_Blank(string target, bool expected)
        {
            // Act
            var result = target.IsNullOrBlank();
            
            // Assert
            result.ShouldBe(expected);
        }
    }
}