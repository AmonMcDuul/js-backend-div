using Core.Entities.SpaceTraderModels;

namespace Api.ViewModels
{
    public class HighScoreResponseModel
    {
        public int Score { get; set; }
        public string Alias { get; set; } = "";
        public GameTypeState GameTypeState { get; set; }
        public HighScoreResponseModel(HighScore highscore) 
        { 
            Score = highscore.Score;
            Alias = highscore.Alias;
            GameTypeState = highscore.GameTypeState;
        }
    }

    public class HighScoreRequest
    {
        public int Score { get; set; }
        public string Alias { get; set; } = "";
        public GameTypeState GameTypeState { get; set; }
    }


}
