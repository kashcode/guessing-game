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
 - when game is started ask for the players name
- Game Screen 
  - 4 inputs in one row - for each input digit one
  - Make Guess button
  - Number of tires left
  - Result of the previous try (previous input and M:m; P:p)
  - Log of previous tries
- Game Over Screen
  - You win / You lose message
  - Secret number
  - New Game button
- Leaderboard
  - rank layers by success rate - correct guesses / games played
  - if the success rate is the same - player with less total tries is ranked higher
  - input minimum games played N - players will be included in leaderbort if at least N games ar played


Technical Requirements:
- Use .NET(C#) or Java
- Game logic must be implemented in the backend
- Validations must be implemented in the frontend and backend
- Implement Unit Tests
- Log all guesses in db (use in-memory db)

Bonus points for:
- Social login ( instead in place of name entering) 
- UI tests
- Responsive UI

Deliveries:
 - Source Code commited to this repository
 - Compiled solution that can be run with single command / scrpipt commited to repository in folder dist
