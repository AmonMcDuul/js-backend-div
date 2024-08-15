using Core.Interfaces;
using Infrastructure.Models;
using System.Collections.Concurrent;

namespace Infrastructure.Services
{
    public class HighScoreCacheService : IHighScoreCacheService
    {
        private readonly ConcurrentBag<HighScoreModel> _highScoresCache;

        public HighScoreCacheService()
        {
            _highScoresCache = new ConcurrentBag<HighScoreModel>();
        }

        public IList<HighScoreModel> GetHighScores()
        {
            return _highScoresCache.ToList();
        }

        public void UpdateCache(IEnumerable<HighScoreModel> highScores)
        {
            _highScoresCache.Clear();
            foreach (var highScore in highScores)
            {
                _highScoresCache.Add(highScore);
            }
        }

        public void AddHighScoreToCache(HighScoreModel highScore)
        {
            _highScoresCache.Add(highScore);
        }
    }

}
