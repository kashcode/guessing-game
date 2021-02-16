namespace GuessingGame.Models
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Rank { get; set; }
        public int TotalGuesses { get; set; }
        public int GamesPlayed { get; set; }
    }
}
