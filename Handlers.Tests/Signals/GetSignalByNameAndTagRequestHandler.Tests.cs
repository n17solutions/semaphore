using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Handlers.Extensions;
using N17Solutions.Semaphore.Handlers.Signals;
using N17Solutions.Semaphore.Requests.Signals;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Handlers.Tests.Signals
{
    public class GetSignalByNameAndTagRequestHandlerTests : IDisposable
    {
        private class TestObject
        {
            public object Value { get; set; }
        }
        
        private const string Name = "Test Signal";
        private const string Value = "Test Value";
        private const string Tags = "Test,Tags";
        
        private readonly Guid _resourceId = Guid.NewGuid();
        
        private readonly SemaphoreContext _context;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly GetSignalByNameAndTagRequestHandler _sut;

        public GetSignalByNameAndTagRequestHandlerTests()
        {
            var dbOptions = new DbContextOptionsBuilder<SemaphoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new SemaphoreContext(dbOptions);

            _context.Signals.Add(new Signal
            {
                Id = 1,
                ResourceId = _resourceId,
                Name = Name,
                Value = Value,
                ValueType = Value.GetSignalValueType(),
                IsBaseType = Value.IsBaseType(),
                Tags = Tags,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            });
            _context.SaveChanges();
            
            _sut = new GetSignalByNameAndTagRequestHandler(_context, _mediatorMock.Object);
        }

        [Fact]
        public async Task Should_Get_Signal_By_Name_And_Tag()
        {
            // Arrange
            var request = new GetSignalByNameAndTagRequest
            {
                Name = Name,
                Tag = "Test"
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            result.ResourceId.ShouldBe(_resourceId);
            result.Name.ShouldBe(Name);
            result.Value.ShouldBe(Value);
        }

        [Fact]
        public async Task Should_Get_Object_Signal_By_Name_And_Tag()
        {
            // Arrange
            var value = new TestObject { Value = Value };
            
            await _context.Signals.AddAsync(new Signal
            {
                Id = 2,
                ResourceId = Guid.NewGuid(),
                Name = $"{Name}_object",
                Value = JsonConvert.SerializeObject(value),
                ValueType = value.GetSignalValueType(),
                IsBaseType = value.IsBaseType(),
                Tags = Tags,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            }).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            
            // Arrange
            var request = new GetSignalByNameAndTagRequest
            {
                Name = $"{Name}_object",
                Tag = "Test"
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            result.Value.ShouldBeOfType<JObject>();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}