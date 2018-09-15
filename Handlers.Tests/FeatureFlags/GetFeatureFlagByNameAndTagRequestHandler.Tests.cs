using System;
using System.Collections.Generic;
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
    public class GetFeatureFlagByNameAndTagRequestHandlerTests : IDisposable
    {
        private const string Name = "Test Feature Flag";
        private const string Tag = "Tag";
        private const bool Value = true;
        
        private readonly Guid _resourceId = Guid.NewGuid();
        
        private readonly GetFeatureFlagByNameAndTagRequestHandler _sut;
        private readonly SemaphoreContext _context;

        public GetFeatureFlagByNameAndTagRequestHandlerTests()
        {
            var dbOptions = new DbContextOptionsBuilder<SemaphoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new SemaphoreContext(dbOptions);
            _context.Features.Add(new Feature
            {
                ResourceId = _resourceId,
                Name = Name,
                Signals = new List<Signal>
                {
                    new Signal
                    {
                        Tags = $"Tag1,Tag2,Tag3,{Tag}",
                        Value = Value.ToString()
                    }
                }
            });
            _context.SaveChanges();
            
            _sut = new GetFeatureFlagByNameAndTagRequestHandler(_context);
        }

        [Fact]
        public async Task Should_Get_FeatureFlag_By_Name_And_Tag()
        {
            // Arrange
            var request = new GetFeatureFlagByNameAndTagRequest
            {
                Name = Name,
                Tag = Tag
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            result.ResourceId.ShouldBe(_resourceId);
            result.Name.ShouldBe(Name);
            result.Setting.ShouldBe(Value);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}