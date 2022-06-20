using Lottery.Models;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Services;

public class DrawingService : BackgroundService
{
    public IServiceProvider Service { get; }

    public DrawingService(IServiceProvider service)
    {
        Service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }


    private async Task DoWork(CancellationToken stoppingToken)
    {
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     executionCount++;
        //
        //     _logger.LogInformation(
        //         "Scoped Processing Service is working. Count: {Count}", executionCount);
        //
        //     await Task.Delay(10000, stoppingToken);
        // }

        using (var scope = Service.CreateScope())
        {
            var scopedProcessingService =
                scope.ServiceProvider
                    .GetRequiredService<IScopedProcessingService>();

            await scopedProcessingService.DoWork(stoppingToken);
        }
    }
}

public interface IScopedProcessingService
{
    Task DoWork(CancellationToken stoppingToken);
}

public class ScopedProcessingService : IScopedProcessingService
{
    private readonly LotteryContext _context;
    public IConfiguration Configuration { get; }

    private int executionCount = 0;
    // private readonly ILogger _logger;

    public ScopedProcessingService(IConfiguration configuration, LotteryContext context)
    {
        _context = context;
        Configuration = configuration;
    }

    public async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var justCreated = false;
            // _logger.LogInformation(
            Console.WriteLine("Scoped Processing Service is working. Count: " + executionCount);

            Status status;
            if (!_context.Status.Any())
            {
                status = new Status
                {
                    DrawInterval = Configuration
                        .GetSection("Config:DrawInterval").Get<int>(),
                    NextDrawId = 1,
                    NextDrawTime = DateTime.Now.AddSeconds(Configuration
                        .GetSection("Config:DrawInterval").Get<int>()),
                    LatestFinishedDrawId = 0
                };
                await _context.AddAsync(status, stoppingToken);
                await _context.Draws.AddAsync(new Draw { DrawDate = status.NextDrawTime, DrawId = status.NextDrawId}, stoppingToken);
                await _context.SaveChangesAsync(stoppingToken);
                justCreated = true;
            }

            status = await _context.Status.FirstOrDefaultAsync(cancellationToken: stoppingToken);

            if (status != null && !justCreated)
            {
                status.LatestFinishedDrawId +=1;
                status.NextDrawId += 1;
                status.NextDrawTime = status.NextDrawTime.AddSeconds(Configuration
                    .GetSection("Config:DrawInterval")
                    .Get<int>());
                status.DrawInterval = Configuration
                    .GetSection("Config:DrawInterval")
                    .Get<int>();
                _context.Status.Update(status);
                await _context.SaveChangesAsync(stoppingToken);


                //Declare drawn
                var draw = await _context.Draws.FirstOrDefaultAsync(x => x.DrawId == status.LatestFinishedDrawId,
                    cancellationToken: stoppingToken);
                if (draw != null)
                {
                    draw.Drawn = true;
                }

                await _context.Draws.AddAsync(new Draw { DrawDate = status.NextDrawTime, DrawId = status.NextDrawId}, stoppingToken);
                await _context.SaveChangesAsync(stoppingToken);

                //Declare winner

                var winningTicket = await _context.Tickets.Where(x => x.DrawId == status.LatestFinishedDrawId)
                    .OrderBy(r => Guid.NewGuid()).FirstOrDefaultAsync(cancellationToken: stoppingToken);

                if (winningTicket != null)
                {
                    winningTicket.HasWin = true;
                    if (draw != null)
                    {
                        draw.WinningSequence = winningTicket.TicketSequence;
                    }
                }

                await _context.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(Configuration
                .GetSection("Config:DrawInterval")
                .Get<int>() * 1000, stoppingToken);
        }
    }
}