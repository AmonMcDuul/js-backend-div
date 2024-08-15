using Core.Entities.SpaceTraderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class HighScoreModel
    {
        public int Score { get; set; }
        public string Alias { get; set; } = "";
        public GameTypeState GameTypeState { get; set; }
        public HighScoreModel(HighScore highscore)
        {
            Score = highscore.Score;
            Alias = highscore.Alias;
            GameTypeState = highscore.GameTypeState;
        }
        public HighScoreModel(int score, string alias, GameTypeState gameTypeState)
        {
            Score = score;
            Alias = alias;
            GameTypeState = gameTypeState;
        }
    }
}
