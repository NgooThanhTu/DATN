using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services
{
    public class AuditLogWorker : BackgroundService
    {
        private readonly IAuditLogQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuditLogWorker> _logger;
        private const int MaxBatchSize = 100;
        private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(10);

        public AuditLogWorker(IAuditLogQueue queue, IServiceScopeFactory scopeFactory, ILogger<AuditLogWorker> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuditLogWorker is starting.");

            var batch = new List<AuditLog>(MaxBatchSize);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    cts.CancelAfter(FlushInterval);

                    try
                    {
                        var itemAvailable = await _queue.WaitToReadAsync(cts.Token);
                        if (!itemAvailable) break; // Channel closed

                        while (batch.Count < MaxBatchSize && _queue.TryDequeue(out var log))
                        {
                            if (log != null) batch.Add(log);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // 10s timeout triggers flush
                    }

                    if (batch.Count > 0 && (batch.Count >= MaxBatchSize || cts.IsCancellationRequested))
                    {
                        await ProcessBatchAsync(batch, stoppingToken);
                        batch.Clear();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in audit log processing.");
                }
            }
        }

        private async Task ProcessBatchAsync(List<AuditLog> logs, CancellationToken cancellationToken)
        {
            if (logs.Count == 0) return;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await dbContext.AuditLogs.AddRangeAsync(logs, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save AuditLogs batch.");
            }
        }
    }
}
