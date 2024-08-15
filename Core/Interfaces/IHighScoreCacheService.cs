using Core.Entities.SpaceTraderModels;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IHighScoreCacheService
    {
        IList<HighScoreModel> GetHighScores();
        void UpdateCache(IEnumerable<HighScoreModel> highScores);
        void AddHighScoreToCache(HighScoreModel highScore);
    }
}
