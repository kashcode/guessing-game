using GuessingGame.Models;
using System.Collections.Generic;

namespace GuessingGame
{
    public interface IGameService
    {        
        public Game CreateGame(Player player);        
        public void MakeGuess(string gameUuid, int[] secret, int[] input);        
        public bool InputIsInRenge(int[] input);
        public Game GetGame(string gameUuid);
        public bool RegisterPlayer(Player player);
        public void RegisterStats(Game game);
        public IEnumerable<Player> GetStats();
        public IEnumerable<Player> GetStats(int gameCount);
    }
}
