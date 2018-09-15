using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Handlers.FeatureFlags;
using N17Solutions.Semaphore.Requests.FeatureFlags;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Handlers.Tests.FeatureFlags
{
    public class GetFeatureFlagByResourceIdRequestHandlerTests : IDisposable
    {
        private const string Name = "Test Feature";
        private const int SignalsCount = 2;
        
        private readonly Guid _resourceId = Guid.NewGuid();
        private readonly SemaphoreContext _context;
        private readonly GetFeatureFlagByResourceIdRequestHandler _sut;

        public GetFeatureFlagByResourceIdRequestHandlerTests()
        {
            var dbOptions = new DbContextOptionsBuilder<SemaphoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new SemaphoreContext(dbOptions);

            var feature = new Feature
            {
                ResourceId = _resourceId,
                Name = Name
            };
            for (var i = 0; i < SignalsCount; i++)
            {
                feature.Signals.Add(new Signal
                {
                    Value = (i % 2 == 0).ToString(),
                    Tags = i.ToString()
                });
            }

            _context.Features.Add(feature);
            _context.SaveChanges();
            
            _sut = new GetFeatureFlagByResourceIdRequestHandler(_context);
        }

        [Fact]
        public async Task Should_Retrieve_FeatureFlag_By_ResourceId()
        {
            // Arrange
            var request = new GetFeatureFlagByResourceIdRequest
            {
                ResourceId = _resourceId
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            result.ResourceId.ShouldBe(_resourceId);
            result.Name.ShouldBe(Name);
            result.Settings.Length.ShouldBe(SignalsCount);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}