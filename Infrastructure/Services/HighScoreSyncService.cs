using Core.Entities.SpaceTraderModels;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class HighScoreSyncService : IHighScoreSyncService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHighScoreCacheService _highScoreCacheService;
        private static int _isSyncing = 0;

        public HighScoreSyncService(IServiceProvider serviceProvider, IHighScoreCacheService highScoreCacheService)
        {
            _highScoreCacheService = highScoreCacheService;
            _serviceProvider = serviceProvider;
        }
            

        public async Task SyncCacheWithDatabaseAsync() 
        {
            if (Interlocked.CompareExchange(ref _isSyncing, 1, 0) == 1)
                return;

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<JsDbContext>();
                    var highScoreCacheService = scope.ServiceProvider.GetRequiredService<IHighScoreCacheService>();

                    var highScores = await context.HighScores.ToListAsync();
                    var groupedAndSortedScores = highScores
                        .GroupBy(h => h.GameTypeState)
                        .SelectMany(g => g.OrderByDescending(h => h.Score))
                        .ToList();

                    var result = groupedAndSortedScores
                        .Select(item => new HighScoreModel(item))
                        .ToList();

                    highScoreCacheService.UpdateCache(result);
                }
            }
            catch
            {
                await Task.Delay(10000);
            }
            finally
            {
                Interlocked.Exchange(ref _isSyncing, 0);
            }
        }

        public async Task SaveHighScoreAsync(HighScore newHighScore)
        {
            bool saved = false;
            while (!saved)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<JsDbContext>();

                        // This retry policy automatically handles transient faults
                        var retryPolicy = context.Database.CreateExecutionStrategy();
                        await retryPolicy.ExecuteAsync(async () =>
                        {
                            context.HighScores.Add(newHighScore);
                            await context.SaveChangesAsync();
                            saved = true;
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

                await Task.Delay(10000);
            }
        }
    }
}
