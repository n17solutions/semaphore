using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Domain.Model;
using N17Solutions.Semaphore.Handlers.Settings;
using N17Solutions.Semaphore.Requests.Settings;
using Shouldly;
using Xunit;

namespace N17Solutions.Semaphore.Handlers.Tests.Settings
{
    public class GetSettingRequestHandlerTests : IDisposable
    {
        private const string Name = "Test Setting", Value = "Test Setting Value";
        private readonly SemaphoreContext _context;
        private readonly GetSettingRequestHandler _sut;

        public GetSettingRequestHandlerTests()
        {
            var dbOptions = new DbContextOptionsBuilder<SemaphoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new SemaphoreContext(dbOptions);
            _context.Add(new Setting
            {
                Name = Name,
                Value = Value
            });
            _context.SaveChanges();
            
            _sut = new GetSettingRequestHandler(_context);
        }

        [Fact]
        public async Task Should_Get_Setting()
        {
            // Act
            var result = await _sut.Handle(new GetSettingRequest
            {
                Name = Name
            }, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldBe(Value);
        }

        [Fact]
        public async Task Should_Return_Null_If_Setting_Doesnt_Exist()
        {
            // Act
            var result = await _sut.Handle(new GetSettingRequest
            {
                Name = "Doesn't Exist"
            }, CancellationToken.None).ConfigureAwait(false);
            
            // Assert
            result.ShouldBeNull();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}