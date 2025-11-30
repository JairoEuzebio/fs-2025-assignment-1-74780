using Microsoft.Extensions.Hosting;
using fs_2025_assignment_1_74780.Services;

namespace fs_2025_assignment_1_74780.Services;

public class DublinBikeBackgroundUpdater : BackgroundService
{
    private readonly IDublinBikeService _service;
    private readonly ILogger<DublinBikeBackgroundUpdater> _logger;

    public DublinBikeBackgroundUpdater(IDublinBikeService service, ILogger<DublinBikeBackgroundUpdater> logger)
    {
        _service = service;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _service.UpdateRandomAvailabilityAsync(stoppingToken);
            _logger.LogInformation("Stations updated at: {time}", DateTimeOffset.Now);
            await Task.Delay(15000, stoppingToken); // 15 seconds
        }
    }
}