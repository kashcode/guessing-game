using System.Collections.Generic;

namespace GuessingGame.Models
{
    public class Game
    {
        public string Uuid { get; set; }
        public int[] Secret { get; set; }
        public int GuessesCount { get; set; }
        public IEnumerable<Guess> Logs { get; set; }
        public int GuessesLeft { 
            get { 
                return 8 - GuessesCount; 
            } 
        }
        public bool SecretGuessed { get; set; }
        public bool GameOver { 
            get {
                return GuessesLeft == 0;
            } 
        }
    }
}
