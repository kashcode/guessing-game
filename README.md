# Guessing Game

Implement Web-based application - guessing game. 

Game rules:
- Program chooses a random secret number with 4 digits.
- All digits in the secret number are different.
- Player has 8 tries to guess the secret number.
- After each guess program displays the message "M:m; P:p" where:
  - m - number of matching digits but not on the right places
  - p - number of matching digits on exact places
- Game ends after 8 tries or if the correct number is guessed.  

Samples:

Secret:  **7046**
Guess:   **8724**
Message: **M:2; P:0**

Secret:  **7046**
Guess:   **7842**
Message: **M:0; P:2**

Secret:  **7046**
Guess:   **7640**
Message: **M:2; P:2**


Game UI Flow

- First Screen - display game rules with start game button at the center of the screen
  - when the game is started player must be asked the name
- _Game_ Screen 
  - Player name
  - 4 inputs in one row - one for each input digit 
  - Make Guess button
  - Number of tries left
  - Result of the previous try (previous input and M:m; P:p)
  - Log of previous tries
- _Game Over_ Screen
  - Player name
  - You win / You lose message
  - Secret number
  - New Game button
- _Leaderboard_ Screen
  - Rank layers by success rate - correct guesses/games played
  - If the success rate is the same - player with fewer total tries is ranked higher
  - Input minimum games played N - players will be included in the leaderboard if at least N games are played


Technical Requirements:
- Use C# or Java
- Game logic must be implemented in the backend
- Validations must be implemented in the frontend and backend
- Implement Unit Tests
- For persistence use in-memory db

Bonus points for:
- Social login ( instead of manual name enter) 
- Functional end to end tests
- Responsive UI

Deliveries:
 - Source Code committed to this repository
 - Compiled solution that can be run with a single command/script committed to the repository in folder dist
