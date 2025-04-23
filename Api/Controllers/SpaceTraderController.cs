using Core.Interfaces;
using Core.Entities.SpaceTraderModels;
using Microsoft.AspNetCore.Mvc;
using Api.ViewModels;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpaceTraderController : ControllerBase
    {
        private readonly JsDbContext _context;

        public SpaceTraderController(
            JsDbContext context)
        {
            _context = context;
        }

        [HttpGet("highscore")]
        public async Task<ActionResult<ICollection<HighScoreResponseModel>>> GetHighScoreAsync()
        {
            var highScores = await _context.HighScores.OrderByDescending(h => h.Score).ToListAsync();
            var result = highScores.Select(h => new HighScoreResponseModel(h));
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
            _context.HighScores.Add(newHighScore);
            await _context.SaveChangesAsync();

            return Ok();
        }


    }
}
