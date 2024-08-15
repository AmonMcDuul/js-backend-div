using Core.Entities.SpaceTraderModels;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class HighScoreSyncService : IHighScoreSyncService
    {
        private readonly JsDbContext _context;
        private readonly IHighScoreCacheService _highScoreCacheService;
        private static int _isSyncing = 0;

        public HighScoreSyncService(JsDbContext context, IHighScoreCacheService highScoreCacheService)
        {
            _context = context;
            _highScoreCacheService = highScoreCacheService;
        }

        public async Task SaveHighScoreAsync(HighScore newHighScore)
        {
            bool saved = false;
            while (!saved)
            {
                try
                {
                    _context.HighScores.Add(newHighScore);
                    await _context.SaveChangesAsync();
                    saved = true;
                }
                catch
                {
                    await Task.Delay(10000);
                }
            }
        }

        public async Task SyncCacheWithDatabaseAsync()
        {
            if (Interlocked.CompareExchange(ref _isSyncing, 1, 0) == 1)
                return; 

            try
            {
                var highScores = await _context.HighScores.ToListAsync();
                var groupedAndSortedScores = highScores
                    .GroupBy(h => h.GameTypeState)
                    .SelectMany(g => g.OrderByDescending(h => h.Score))
                    .ToList();

                var result = groupedAndSortedScores
                    .Select(item => new HighScoreModel(item))
                    .ToList();

                _highScoreCacheService.UpdateCache(result);
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
    }
}
