using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Handlers.Extensions;
using N17Solutions.Semaphore.Handlers.Signals;
using N17Solutions.Semaphore.Requests.Security;
using N17Solutions.Semaphore.Requests.Signals;
using N17Solutions.Semaphore.ServiceContract;
using N17Solutions.Semaphore.ServiceContract.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Handlers.Tests.Signals
{
    public class GetSignalByNameRequestHandlerTests : IDisposable
    {
        private class TestObject
        {
            public object Value { get; set; }
        }
        
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
                ValueType = Value.GetSignalValueType(),
                IsBaseType = Value.IsBaseType(),
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
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            }).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            
            // Arrange
            var request = new GetSignalByNameRequest
            {
                Name = $"{Name}_object"
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            result.Value.ShouldBeOfType<TestObject>();
        }
        
        [Fact]
        public async Task Should_Get_Encrypted_Object_Signal_With_No_PrivateKey()
        {
            // Arrange
            var value = new TestObject { Value = Value };
            await _context.Signals.AddAsync(new Signal
            {
                Id = 3,
                ResourceId = Guid.NewGuid(),
                Name = $"{Name}_object_encrypted",
                Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value))),
                ValueType = value.GetSignalValueType(),
                IsBaseType = value.IsBaseType(),
                Tags = Constants.EncryptedTag,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            }).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            var request = new GetSignalByNameRequest
            {
                Name = $"{Name}_object_encrypted"
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            result.Value.ShouldBeOfType<string>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Should_Only_Call_For_Decryption_If_Signal_Is_Marked_As_Encrypted(bool isEncrypted)
        {
            // Arrange
            var value = new TestObject { Value = Value };
            await _context.Signals.AddAsync(new Signal
            {
                Id = 3,
                ResourceId = Guid.NewGuid(),
                Name = $"{Name}_object_encrypted",
                Value = isEncrypted
                    ? Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)))
                    : JsonConvert.SerializeObject(value),
                ValueType = value.GetSignalValueType(),
                IsBaseType = value.IsBaseType(),
                Tags = isEncrypted ? Constants.EncryptedTag : string.Empty,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now
            }).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            
            _mediatorMock.Setup(x => x.Send(It.IsAny<DecryptionRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(JsonConvert.SerializeObject(value));
            
            var request = new GetSignalByNameRequest
            {
                Name = $"{Name}_object_encrypted",
                PrivateKey = "Private Key"
            };
            
            // Act
            await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            _mediatorMock.Verify(x => x.Send(It.IsAny<DecryptionRequest>(), It.IsAny<CancellationToken>()), () => isEncrypted ? Times.Once() : Times.Never());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}