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

        [HttpPost("addGame")]
        public async Task<IActionResult> CreateGame(CreateGameRequest createGame)
        {
            var game = await _gameService.CreateGame(createGame);
            return Created("",game);
        }

        [HttpGet("games")]
        public async Task<IActionResult> GetAllGames()
        {
            return Ok(new GamesResponse
            {
                Success = true,
                Games = await _gameService.GetGames()
			});
        }

		[HttpGet("games/{category}")]
		public async Task<IActionResult> GetAllGames(int category)
		{
			return Ok(new GamesResponse
			{
                Success = true,
                Games = await _gameService.GetGamesByCategory(category)
			});
		}

        [HttpPost("getSession")]
        public async Task<IActionResult> GetSessionForUser(UserSessionRequest request)
        {
            Guid gameSessionId = await _gameService.GetSessionForUser(request);

			return Ok(new HttpResponseModel
            {
                Success = true,
                Message = gameSessionId
            });
        }

        [HttpGet("games/ended/{gameSessionId}")]
        public async Task<IActionResult> CheckIfGameEnded(Guid gameSessionId)
        {
            bool isEnded = await _gameService.CheckIfGameEnded(gameSessionId);

			return Ok(new HttpResponseModel
            {
                Success = isEnded
            });
        }

        [HttpPut("adm/games/update")]
        public async Task<IActionResult> UpdateIsGameActive(AdminGameChangeActiveRequest request)
        {
            await _gameService.UpdateIsGameActive(request);

            return Ok(new HttpResponseModel
            {
                Success = true
            });
        }

		[HttpGet("adm/games")]
		public async Task<IActionResult> GetAllGamesAdmin()
		{
			var games = await _gameService.GetGamesAdmin();

			return Ok(new GamesResponse
			{
				Success = true,
                Games = games
			});
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

                        var sessions = await _gameService.EndAllActiveSessions(new UserSessionRequest
                        {
                            UserId = startGame.UserId,
                            UserSessionId = startGame.UserSessionId,
                            GameType = startGame.Type
                        });

                        if (sessions.Any())
                        {
                            foreach (var session in sessions)
                            {
                                var historyId = await _gameService.CreateGameHistoryRecord(session);
                                session.GameHistoryId = historyId;
                                await _gameService.SaveSession(session);
                            }
                        }

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

                        if(resp == null)
                        {
                            return Ok(new HttpResponseModel
                            {
                                Success = false,
                                Error = "Something went wrong while making move"
                            });
                        }

						await _gameService.CreateGameAction(startGame, session.GameSessionId);
                        var dict = new Dictionary<string, object>();
                        dict["Status"] = resp.Message.Status;
                        dict["Multiplier"] = resp.Message.Multiplier;
                        if (resp.Message.Data != null)
                        {
							foreach (var kv in resp.Message.Data)
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
                        session.Game = game;
                        session.CurrentMultiplier = game.GetMultiplier();
                        session.Result = game.GetStatus() == GameStatus.InProgress ? null : game.GetStatus() == GameStatus.EndedLose ? ResultType.Lost : ResultType.Won;
                        session.CashWon = game.GetWinnedAmount();
                        

                        if(resp.Message.Status==GameStatus.EndedLose || resp.Message.Status == GameStatus.EndedWin)
                        {
                            session.EndTime = DateTime.Now.AddHours(1);
                            var historyId = await _gameService.CreateGameHistoryRecord(session);
                            session.GameHistoryId = historyId;
                        }

						await _gameService.SaveSession(session);

						return Ok(resp);
                    }
                case ActionType.End:
                    {
						GameSession session = await _gameService.GetGameSession((Guid)startGame.GameSessionId);
						AbstractGame game = session.Game;
						await _gameService.CreateGameAction(startGame, session.GameSessionId);
						var resp = _gameService.CashOutEarly(game);

                        if(resp == null)
                        {
							return Ok(new HttpResponseModel
							{
								Success = false,
								Error = "Wrong game type for cashout"
							});
						}

                        if (resp.Success)
                        {
                            session.CashedOutEarly = true;
                            session.EndTime = DateTime.UtcNow.AddHours(1);
							session.Game = game;
							session.CurrentMultiplier = game.GetMultiplier();
							session.Result = game.GetStatus() == GameStatus.InProgress ? null : game.GetStatus() == GameStatus.EndedLose ? ResultType.Lost : ResultType.Won;
							session.CashWon = game.GetWinnedAmount();
                            session.Status = game.GetStatus();
                            await _gameService.SaveSession(session);

						}
						await _gameService.CreateGameHistoryRecord(session);
						var dict = new Dictionary<string, object>();
						dict["Status"] = resp.Message.Status;
						dict["Multiplier"] = resp.Message.Multiplier;
                        if(resp.Message.Data != null)
                        {
							foreach (var kv in resp.Message.Data)
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
						return Ok(resp);
					}
                default:
					return Ok(new HttpResponseModel
					{
						Success = false,
						Error = "Wrong action type or no action type was given"
					});
				
			}
        }

    }
}
