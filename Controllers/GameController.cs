using game_service.classes;
using game_service.models;
using game_service.models.DTOs;
using game_service.services;
using Microsoft.AspNetCore.Mvc;

namespace game_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessGame(ProcessGameRequest startGame)
        {
            switch (startGame.Action)
            {
                case ActionType.Start: {
                        AbstractGame game = _gameService.GetGame(startGame);
                        game.InicializeGame(startGame.Data);
                        Guid sessionId = await _gameService.CreateGameSession(game, startGame.UserId, startGame.UserSessionId);
                        await _gameService.CreateGameAction(startGame, sessionId);

                        return Ok(new HttpResponseModel
                        {
                            Success = true,
                            Message = sessionId
                        });
                    }
                case ActionType.Move: {
                        GameSession session = await _gameService.GetGameSession((Guid)startGame.GameSessionId);
                        AbstractGame game = session.Game;
                        
                    } break;
                case ActionType.End: break;
                case ActionType.System: break;
            }
        }

    }
}
