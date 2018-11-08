using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Handlers.Signals;
using N17Solutions.Semaphore.Requests.Security;
using N17Solutions.Semaphore.Requests.Settings;
using N17Solutions.Semaphore.Requests.Signals;
using N17Solutions.Semaphore.Responses.Signals;
using N17Solutions.Semaphore.ServiceContract;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Handlers.Tests.Signals
{
    public class CreateSignalRequestHandlerTests : IDisposable
    {
        private readonly SemaphoreContext _context;
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly CreateSignalRequestHandler _sut;

        public CreateSignalRequestHandlerTests()
        {
            var dbOptions = new DbContextOptionsBuilder<SemaphoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new SemaphoreContext(dbOptions);

            _sut = new CreateSignalRequestHandler(_context, _mediatorMock.Object);
        }

        [Fact]
        public async Task Should_Create_New_String_Signal()
        {
            // Arrange
            var request = new CreateSignalRequest
            {
                Value = "Test Signal",
                Tags = new List<string> {"Test", "Tags"}
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            (await _context.Signals.FirstOrDefaultAsync(signal => signal.ResourceId == result).ConfigureAwait(false)).ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Create_New_Object_Signal()
        {
            // Arrange
            var request = new CreateSignalRequest
            {
                Value = new
                {
                    Property = "Property",
                    Value = 1
                }
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            (await _context.Signals.FirstOrDefaultAsync(signal => signal.ResourceId == result).ConfigureAwait(false)).ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Create_New_Signal_With_No_Tags()
        {
            // Arrange
            var request = new CreateSignalRequest
            {
                Value = "Test Signal"
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            (await _context.Signals.FirstOrDefaultAsync(signal => signal.ResourceId == result).ConfigureAwait(false)).ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Create_New_Encrypted_Signal()
        {
            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<EncryptionRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((EncryptionRequest encryptionRequest, CancellationToken token) 
                => Convert.ToBase64String(Encoding.UTF8.GetBytes(encryptionRequest.ToEncrypt)));
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetSettingRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes("Test Public Key")));
            
            var request = new CreateSignalRequest
            {
                Value = "Test Signal",
                Tags = new List<string> {"Test", "Tags"},
                Encrypted = true
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            var signal = await _context.Signals.FirstOrDefaultAsync(s => s.ResourceId == result).ConfigureAwait(false);
            signal.ShouldNotBeNull();
            signal.Value.ShouldNotBeNull();
            signal.Tags.ShouldContain(Constants.EncryptedTag);
        }

        [Fact]
        public async Task Should_Throw_If_Already_Exists()
        {
            // Arrange
            var signalResponse = new SignalResponse();
            var request = new CreateSignalRequest
            {
                Value = "Test Signal"
            };
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetSignalByNameRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(signalResponse);
            
            // Act
            var result = await Should.ThrowAsync<InvalidOperationException>(_sut.Handle(request, CancellationToken.None)).ConfigureAwait(false);
            
            // Assert
            result.Message.ShouldContain(CreateSignalRequestHandler.SignalAlreadyExistsErrorMessage);
        }
        
        [Fact]
        public async Task Should_Throw_If_Already_Exists_With_Tags()
        {
            // Arrange
            var signalResponse = new SignalResponse();
            var request = new CreateSignalRequest
            {
                Value = "Test Signal",
                Tags = new List<string>{"tag"}
            };
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetSignalByNameAndTagRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(signalResponse);
            
            // Act
            var result = await Should.ThrowAsync<InvalidOperationException>(_sut.Handle(request, CancellationToken.None)).ConfigureAwait(false);
            
            // Assert
            result.Message.ShouldContain(CreateSignalRequestHandler.SignalAlreadyExistsErrorMessage);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}