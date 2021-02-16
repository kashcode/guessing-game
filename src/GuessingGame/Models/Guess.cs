namespace GuessingGame.Models
{
    public class Guess
    {
        public string Input { get; set; }
        public int MatchingDigits { get; set; }
        public int MatchingDigitsPlaces { get; set; }
        public override string ToString()
        {
            return $"{Input} M:{MatchingDigits} P:{MatchingDigitsPlaces}";
        }
    }
}
