using System;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.BackgroundTasks.Background
{
    public interface IScopedProcessingService
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
