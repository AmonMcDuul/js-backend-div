using Core.Entities.SpaceTraderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IHighScoreSyncService
    {
        Task SyncCacheWithDatabaseAsync();
        Task SaveHighScoreAsync(HighScore newHighScore);
    }
}
