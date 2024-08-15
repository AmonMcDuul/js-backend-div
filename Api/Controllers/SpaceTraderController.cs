using Core.Interfaces;
using Core.Entities.SpaceTraderModels;
using Microsoft.AspNetCore.Mvc;
using Api.ViewModels;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Infrastructure.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpaceTraderController : ControllerBase
    {
        private readonly JsDbContext _context;
        private readonly IHighScoreCacheService _highScoreCacheService;
        private readonly IHighScoreSyncService _highScoreSyncService;

        public SpaceTraderController(
            JsDbContext context,
            IHighScoreCacheService highScoreCacheService,
            IHighScoreSyncService highScoreSyncService)
        {
            _context = context;
            _highScoreCacheService = highScoreCacheService;
            _highScoreSyncService = highScoreSyncService;
        }

        [HttpGet("highscore")]
        public ActionResult<ICollection<HighScoreResponseModel>> GetHighScore()
        {
            var cachedScores = _highScoreCacheService.GetHighScores();
            if (cachedScores.Any())
            {
                _highScoreSyncService.SyncCacheWithDatabaseAsync();
                return Ok(cachedScores);
            }

            var highScores = _context.HighScores.ToList();
            var groupedAndSortedScores = highScores
                .GroupBy(h => h.GameTypeState)
                .SelectMany(g => g.OrderByDescending(h => h.Score))
                .ToList();

            var result = groupedAndSortedScores
                .Select(item => new HighScoreResponseModel(item))
                .ToList();  

            var cachedResults = result
                .Select(item => new HighScoreModel(item.Score, item.Alias, item.GameTypeState))
                .ToList();

            _highScoreCacheService.UpdateCache(cachedResults);
            return Ok(result);
        }

        [HttpPost("highscore")]
        public async Task<ActionResult> PostHighScore([FromBody] HighScoreRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newHighScore = new HighScore(request.Score, request.Alias, request.GameTypeState);
            var newHighScoreResponse = new HighScoreResponseModel(newHighScore);
            var cachedResponse = new HighScoreModel(newHighScore);
            _highScoreCacheService.AddHighScoreToCache(cachedResponse);

            Task.Run(() => _highScoreSyncService.SaveHighScoreAsync(newHighScore));

            return Ok();
        }
    }
}
