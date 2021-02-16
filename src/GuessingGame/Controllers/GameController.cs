using GuessingGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace GuessingGame.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameService _gameService;

        public GameController(ILogger<GameController> logger, IGameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        public IActionResult Index()
        {
            var player = new Player()
            {
                Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Name = User.Identity.Name
            };

            _ = _gameService.RegisterPlayer(player);

            var game = _gameService.CreateGame(player);
            
            return View(game);
        }        

        [HttpPost]
        public IActionResult MakeGuess(string gameUuid, int[] input)
        {
            if (gameUuid == null)
                throw new ArgumentNullException(nameof(gameUuid));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length != 4)
                throw new ArgumentException("Wrong input");

            if (!_gameService.InputIsInRenge(input))
                throw new ArgumentException("Wrong input");

            var game = _gameService.GetGame(gameUuid);

            if (game.GuessesLeft > 0)
            {
                _gameService.MakeGuess(gameUuid, game.Secret, input);

                game = _gameService.GetGame(gameUuid);
            }

            return new JsonResult(new {
                game.GuessesLeft,
                game.Logs,
                game.SecretGuessed,
                game.GameOver,
                url = Url.Action("GameOver", "Game", new { gameUuid = game.Uuid })
            }) {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public IActionResult GameOver(string gameUuid)
        {
            var game = _gameService.GetGame(gameUuid);

            _gameService.RegisterStats(game);

            return View(game);
        }
    }
}
