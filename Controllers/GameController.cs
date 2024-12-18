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
                case ActionType.Start:
                    {
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
                case ActionType.Move:
                    {
                        GameSession session = await _gameService.GetGameSession((Guid)startGame.GameSessionId);
                        AbstractGame game = session.Game;
                        var resp = _gameService.MakeMove(game, startGame.Data);
						await _gameService.CreateGameAction(startGame, session.GameSessionId);
                        var dict = new Dictionary<string, object>();
                        dict["Status"] = resp.Status;
                        dict["Multiplier"] = resp.Multiplier;
                        if (resp.Data != null)
                        {
							foreach (var kv in resp.Data)
							{
								dict[kv.Key] = kv.Value;
							}
						}
                        
						await _gameService.CreateGameAction(new ProcessGameRequest
                        {
                            Type = startGame.Type,
                            UserId = startGame.UserId,
                            UserSessionId = session.UserSessionId,
                            GameSessionId = session.GameSessionId,
                            Action = ActionType.System,
                            BetAmount = startGame.BetAmount,
                            Data = dict

                        }, session.GameSessionId);
						return Ok(new HttpResponseModel
                        {
                            Success = true,
                            Message = resp
                        });
                    }
                case ActionType.End:
                    {
						GameSession session = await _gameService.GetGameSession((Guid)startGame.GameSessionId);
						AbstractGame game = session.Game;
						await _gameService.CreateGameAction(startGame, session.GameSessionId);
						var resp = _gameService.CashOutEarly(game, startGame.Data);
						var dict = new Dictionary<string, object>();
						dict["Status"] = resp.Status;
						dict["Multiplier"] = resp.Multiplier;
						foreach (var kv in resp.Data)
						{
							dict[kv.Key] = kv.Value;
						}
						await _gameService.CreateGameAction(new ProcessGameRequest
						{
							Type = startGame.Type,
							UserId = startGame.UserId,
							UserSessionId = session.UserSessionId,
							GameSessionId = session.GameSessionId,
							Action = ActionType.System,
							BetAmount = startGame.BetAmount,
							Data = dict

						}, session.GameSessionId);
						return Ok(new HttpResponseModel
						{
							Success = true,
							Message = resp
						});
					}
                default:
					return Ok(new HttpResponseModel
					{
						Success = false,
						Error = "No action type was given"
					});
					//case ActionType.System: break;
			}
        }

    }
}
