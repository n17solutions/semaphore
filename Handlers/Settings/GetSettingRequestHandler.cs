using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using N17Solutions.Semaphore.Data.Context;
using N17Solutions.Semaphore.Requests.Settings;

namespace N17Solutions.Semaphore.Handlers.Settings
{
    public class GetSettingRequestHandler : IRequestHandler<GetSettingRequest, string>
    {
        private readonly SemaphoreContext _context;

        public GetSettingRequestHandler(SemaphoreContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(GetSettingRequest request, CancellationToken cancellationToken)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Name.Equals(request.Name), cancellationToken).ConfigureAwait(false);
            return setting?.Value;
        }
    }
}