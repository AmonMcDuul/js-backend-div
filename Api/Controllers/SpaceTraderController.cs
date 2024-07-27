using Api.ViewModels;
using Core.Entities.SpaceTraderModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpaceTraderController : ControllerBase
    {
        private readonly JsDbContext _context;

        public SpaceTraderController(JsDbContext context)
        {
            _context = context;
        }

        [HttpGet("highscore")]
        public ActionResult<ICollection<HighScoreResponseModel>> GetHighScore()
        {
            var highScores = _context.HighScores.ToList();
            var groupedAndSortedScores = highScores
                .GroupBy(h => h.GameTypeState)
                .SelectMany(g => g.OrderByDescending(h => h.Score))
                .ToList();

            var result = new List<HighScoreResponseModel>();
            foreach (var item in groupedAndSortedScores) 
            {
                result.Add(new HighScoreResponseModel(item));
            }
            return Ok(result);
        }

        [HttpPost("highscore")]
        public ActionResult PostHighScore([FromBody] HighScoreRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HighScore newHighScore = new HighScore(request.Score, request.Alias, request.GameTypeState);
            _context.HighScores.Add(newHighScore);
            _context.SaveChanges();
            return Ok();
        }
    }
}
