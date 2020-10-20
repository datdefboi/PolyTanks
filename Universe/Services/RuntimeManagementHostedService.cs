using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Server.Services
{
    public class RuntimeManagementHostedService : IHostedService
    {
        private readonly RoomRuntimeService _runtimeService;

        public RuntimeManagementHostedService(RoomRuntimeService runtimeService)
        {
            _runtimeService = runtimeService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _runtimeService.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}