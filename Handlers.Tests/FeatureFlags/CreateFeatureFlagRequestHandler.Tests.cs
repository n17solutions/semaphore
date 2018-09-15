using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Handlers.FeatureFlags;
using N17Solutions.Semaphore.Requests.FeatureFlags;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Handlers.Tests.FeatureFlags
{
    public class CreateFeatureFlagRequestHandlerTests : IDisposable
    {
        private readonly CreateFeatureFlagRequestHandler _sut;
        private readonly SemaphoreContext _context;

        public CreateFeatureFlagRequestHandlerTests()
        {
            var dbOptions = new DbContextOptionsBuilder<SemaphoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new SemaphoreContext(dbOptions);
            
            _sut = new CreateFeatureFlagRequestHandler(_context);
        }

        [Fact]
        public async Task Should_Create_New_FeatureFlag()
        {
            // Arrange
            var request = new CreateFeatureFlagRequest
            {
                Name = "Test Feature Flag",
                Tags = new[] {"Test", "Flag"},
                Setting = true
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            (await _context.Features.FirstOrDefaultAsync(feature => feature.ResourceId == result).ConfigureAwait(false)).ShouldNotBeNull();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}