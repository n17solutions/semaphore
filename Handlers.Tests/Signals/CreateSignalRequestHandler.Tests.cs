using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Handlers.Security;
using N17Solutions.Semaphore.Handlers.Signals;
using N17Solutions.Semaphore.Requests.Security;
using N17Solutions.Semaphore.Requests.Signals;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Handlers.Tests.Signals
{
    public class CreateSignalRequestHandlerTests : IDisposable
    {
        private readonly SemaphoreContext _context;
        private readonly CreateSignalRequestHandler _sut;

        public CreateSignalRequestHandlerTests()
        {
            var dbOptions = new DbContextOptionsBuilder<SemaphoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new SemaphoreContext(dbOptions);

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(x => x.Send(It.IsAny<EncryptionRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((EncryptionRequest request, CancellationToken token) 
                => Convert.ToBase64String(Encoding.UTF8.GetBytes(request.ToEncrypt)));
            
            _sut = new CreateSignalRequestHandler(_context, mediatorMock.Object);
        }

        [Fact]
        public async Task Should_Create_New_Signal()
        {
            // Arrange
            var request = new CreateSignalRequest
            {
                Value = "Test Signal",
                Tags = new[] {"Test", "Tags"}
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
            var publicKey = Encoding.UTF8.GetBytes("Test Public Key");
            using (var fileStream = File.Create(GenerateKeysRequestHandler.PublicKeyFileName))
                fileStream.Write(publicKey, 0, publicKey.Length);
            var request = new CreateSignalRequest
            {
                Value = "Test Signal",
                Tags = new[] {"Test", "Tags"},
                Encrypted = true
            };
            
            // Act
            var result = await _sut.Handle(request, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldNotBeNull();
            var signal = await _context.Signals.FirstOrDefaultAsync(s => s.ResourceId == result).ConfigureAwait(false);
            signal.ShouldNotBeNull();
            signal.Value.ShouldNotBe(request.Value);
        }

        public void Dispose()
        {
            File.Delete(GenerateKeysRequestHandler.PublicKeyFileName);
            
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}