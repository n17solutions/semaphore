using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Handlers.Signals;
using N17Solutions.Semaphore.Requests.Signals;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Handlers.Tests.Signals
{
    public class GetSignalByNameRequestHandlerTests : IDisposable
    {
        private const string Name = "Test Signal";
        private const string Value = "Test Value";
        
        private readonly Guid _resourceId = Guid.NewGuid();
        
        private readonly SemaphoreContext _context;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly GetSignalByNameRequestHandler _sut;

        public GetSignalByNameRequestHandlerTests()
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
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            });
            _context.SaveChanges();
            
            _sut = new GetSignalByNameRequestHandler(_context, _mediatorMock.Object);
        }
        
        [Fact]
        public async Task Should_Get_Signal_By_Name()
        {
            // Arrange
            var request = new GetSignalByNameRequest
            {
                Name = Name
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            result.ResourceId.ShouldBe(_resourceId);
            result.Name.ShouldBe(Name);
            result.Value.ShouldBe(Value);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}