using Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.SpaceTraderModels
{
    public class HighScore : BaseEntity
    {
        public int Score { get; private set; }
        public string Alias { get; private set; } = "";
        public GameTypeState GameTypeState { get; private set; }
        protected HighScore() { }
        public HighScore(int score, string alias, GameTypeState gameTypeState) 
        { 
            Score = score;
            Alias = alias;
            GameTypeState = gameTypeState;
        }
    }
}
