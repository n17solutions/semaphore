using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Moq;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Handlers.Signals;
using N17Solutions.Semaphore.Requests.Security;
using N17Solutions.Semaphore.Requests.Settings;
using N17Solutions.Semaphore.Requests.Signals;
using N17Solutions.Semaphore.ServiceContract;
using N17Solutions.Semaphore.ServiceContract.Signals;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Handlers.Tests.Signals
{
    public class PatchSignalRequestHandlerTests : IDisposable
    {
        private readonly SemaphoreContext _context;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly PatchSignalRequestHandler _sut;

        public PatchSignalRequestHandlerTests()
        {
            var dbOptions = new DbContextOptionsBuilder<SemaphoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            
            _context = new SemaphoreContext(dbOptions);
            _sut = new PatchSignalRequestHandler(_context, _mediatorMock.Object);
        }

        [Fact]
        public async Task Should_Throw_If_Signal_Not_Found()
        {
            // Arrange
            var id = Guid.NewGuid();
            
            // Act
            var result = await Should.ThrowAsync<InvalidOperationException>(() => _sut.Execute(new PatchSignalRequest
            {
                Id = id
            }, CancellationToken.None)).ConfigureAwait(false);
            
            // Assert
            result.Message.ShouldContain(string.Format(PatchSignalRequestHandler.SignalNotFoundMessage, id));
        }

        [Fact]
        public async Task Should_Update_Signal_With_New_Unencrypted_Base_Object()
        {
            // Arrange
            var id = Guid.NewGuid();
            const string newValue = "New Test Value";
            
            await _context.Signals.AddAsync(new Signal
            {
                Id = 1,
                DateCreated = DateTime.UtcNow,
                DateLastUpdated = DateTime.UtcNow,
                IsBaseType = true,
                Name = "Test",
                ResourceId = id,
                Value = "Test Value",
                ValueType = typeof(string).FullName
            }).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            var request = new PatchSignalRequest
            {
                Id = id,
                Patch = new JsonPatchDocument<SignalWriteModel>()
                    .Replace(x => x.Tags, new[] {"Tag1", "Tag2"})
                    .Replace(x => x.Value, newValue)
            };
            
            // Act
            await _sut.Execute(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            var signal = await _context.Signals.FirstOrDefaultAsync(s => s.ResourceId == id).ConfigureAwait(false);
            signal.Value.ShouldBe(newValue);
            signal.Tags.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Update_Signal_With_New_Encrypted_Base_Object()
        {
            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<EncryptionRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((EncryptionRequest encryptionRequest, CancellationToken token) 
                => Convert.ToBase64String(Encoding.UTF8.GetBytes(encryptionRequest.ToEncrypt)));
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetSettingRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes("Test Public Key")));
            
            var id = Guid.NewGuid();
            const string oldValue = "Test Value";
            const string newValue = "New Test Value";
            
            await _context.Signals.AddAsync(new Signal
            {
                Id = 1,
                DateCreated = DateTime.UtcNow,
                DateLastUpdated = DateTime.UtcNow,
                IsBaseType = true,
                Name = "Test",
                ResourceId = id,
                Value = oldValue,
                ValueType = typeof(string).FullName,
                Tags = Constants.EncryptedTag
            }).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            var request = new PatchSignalRequest
            {
                Id = id,
                Patch = new JsonPatchDocument<SignalWriteModel>()
                    .Replace(x => x.Tags, new[] {"Tag1", "Tag2"})
                    .Replace(x => x.Value, newValue)
            };
            
            // Act
            await _sut.Execute(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            var signal = await _context.Signals.FirstOrDefaultAsync(s => s.ResourceId == id).ConfigureAwait(false);
            signal.Value.ShouldNotBe(newValue);
            signal.Value.ShouldNotBe(oldValue);
            signal.Tags.ShouldNotBeNull();
            signal.Tags.ShouldContain(Constants.EncryptedTag);
        }
        
        [Fact]
        public async Task Should_Update_Signal_With_New_Unencrypted_Object()
        {
            // Arrange
            var id = Guid.NewGuid();
            var newValue = new SignalWriteModel {Name = "Test Name"};
            
            await _context.Signals.AddAsync(new Signal
            {
                Id = 1,
                DateCreated = DateTime.UtcNow,
                DateLastUpdated = DateTime.UtcNow,
                IsBaseType = true,
                Name = "Test",
                ResourceId = id,
                Value = "Test Value",
                ValueType = typeof(string).FullName
            }).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            var request = new PatchSignalRequest
            {
                Id = id,
                Patch = new JsonPatchDocument<SignalWriteModel>()
                    .Replace(x => x.Value, newValue)
            };
            
            // Act
            await _sut.Execute(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            var signal = await _context.Signals.FirstOrDefaultAsync(s => s.ResourceId == id).ConfigureAwait(false);
            signal.Value.ShouldBe(JsonConvert.SerializeObject(newValue));
        }
        
        [Fact]
        public async Task Should_Update_Signal_With_New_Encrypted_Object()
        {
            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<EncryptionRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((EncryptionRequest encryptionRequest, CancellationToken token) 
                => Convert.ToBase64String(Encoding.UTF8.GetBytes(encryptionRequest.ToEncrypt)));
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetSettingRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes("Test Public Key")));
            
            var id = Guid.NewGuid();
            const string oldValue = "Test Value";
            var newValue = new SignalWriteModel {Name = "Test Name"};
            
            await _context.Signals.AddAsync(new Signal
            {
                Id = 1,
                DateCreated = DateTime.UtcNow,
                DateLastUpdated = DateTime.UtcNow,
                IsBaseType = true,
                Name = "Test",
                ResourceId = id,
                Value = oldValue,
                ValueType = typeof(string).FullName,
                Tags = Constants.EncryptedTag
            }).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            var request = new PatchSignalRequest
            {
                Id = id,
                Patch = new JsonPatchDocument<SignalWriteModel>()
                    .Replace(x => x.Tags, new[] {"Tag1", "Tag2"})
                    .Replace(x => x.Value, newValue)
            };
            
            // Act
            await _sut.Execute(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            var signal = await _context.Signals.FirstOrDefaultAsync(s => s.ResourceId == id).ConfigureAwait(false);
            signal.Value.ShouldNotBe(JsonConvert.SerializeObject(newValue));
            signal.Value.ShouldNotBe(oldValue);
            signal.Tags.ShouldNotBeNull();
            signal.Tags.ShouldContain(Constants.EncryptedTag);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}