using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Configuration;

namespace TaskManagement.Infrastructure.BackgroundJobs;

// Runs in the background for as long as the app runs and permanently deletes expired trash.
public class TrashPurgeService : BackgroundService
{
private readonly IServiceScopeFactory _scopeFactory; // lets us create a fresh scope per run
private readonly ProjectSettings _settings;
private readonly ILogger<TrashPurgeService> _logger;

public TrashPurgeService(
IServiceScopeFactory scopeFactory,
IOptions<ProjectSettings> options,
ILogger<TrashPurgeService> logger)
{
_scopeFactory = scopeFactory;
_settings = options.Value;
_logger = logger;
}

// Called once when the app starts.
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{

using var timer = new PeriodicTimer(TimeSpan.FromHours(_settings.PurgeIntervalHours));

try
{
do
{
await PurgeAsync(stoppingToken); // run now, then after each tick
}
while (await timer.WaitForNextTickAsync(stoppingToken)); // false/exception when the app is stopping
}
catch (OperationCanceledException)
{
// The app is shutting down.
}
}

private async Task PurgeAsync(CancellationToken cancellationToken)
{
try
{
//  this service is a SINGLETON.
//  each run creates its own scope to get a fresh repository and DbContext.
using var scope = _scopeFactory.CreateScope();
var repository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();

// Anything trashed before this moment has passed its retention period.
var cutoff = DateTime.UtcNow.AddDays(-_settings.TrashRetentionDays);

var removed = await repository.PurgeExpiredAsync(cutoff, cancellationToken);
_logger.LogInformation("Trash purge removed {Count} project(s).", removed);
}
catch (OperationCanceledException)
{
throw; // ExecuteAsync handles shutdown
}
catch (Exception ex)
{

_logger.LogError(ex, "Trash purge failed.");
}
}
}