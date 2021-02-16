using Microsoft.AspNetCore.Mvc;

namespace GuessingGame.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly IGameService _gameService;

        public LeaderboardController(IGameService gameService)
        {
            _gameService = gameService;
        }

        public IActionResult Index()
        {
            var stats = _gameService.GetStats();

            ViewData["stats"] = stats;

            return View();
        }

        public IActionResult GetStats(int gameCount)
        {
            var result = _gameService.GetStats(gameCount);

            return new JsonResult(result);
        }
    }
}
