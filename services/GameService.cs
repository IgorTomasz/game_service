using Azure.Core;
using game_service.classes;
using game_service.classes.games;
using game_service.models;
using game_service.models.DTOs;
using game_service.repositories;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace game_service.services
{
	public interface IGameService
	{
		public Task<Guid> CreateGameSession(AbstractGame game, Guid userId, Guid userSessionId);
		public AbstractGame GetGame(ProcessGameRequest request);
		public Task CreateGameAction(ProcessGameRequest request, Guid gameSessionId);
		public Task<GameSession> GetGameSession(Guid gameSessionId);
		public AbstractGame RestoreGame(GameData request);
		public GameActionResponse MakeMove(AbstractGame game, Dictionary<string, object> data);
		public GameActionResponse CashOutEarly(AbstractGame game, Dictionary<string, object> data);
		public Task SaveSession(GameSession gameSession);
		public Task<List<Games>> GetGames();
		public Task<List<Games>> GetGamesByCategory(int category);
		public Task<Games> CreateGame(CreateGameRequest createGame);
		public Task<bool> CheckIfGameEnded(Guid gameSessionId);
		public Task<Guid> GetSessionForUser(UserSessionRequest request);
		public Task CreateGameHistoryRecord(GameSession gameSession);
		public Task UpdateIsGameActive(AdminGameChangeActiveRequest request);
	}

	public class GameService : IGameService
	{
		private readonly IGameRepository _gameRepository;

		public GameService(IGameRepository gameRepository)
		{
			_gameRepository = gameRepository;
		}

		public async Task<Guid> CreateGameSession(AbstractGame game, Guid userId, Guid userSessionId)
		{
			return await _gameRepository.CreateGameSession(game, userId, userSessionId);
		}

		public async Task<GameSession> GetGameSession(Guid gameSessionId)
		{
			return await _gameRepository.GetGameSession(gameSessionId);
		}

		public AbstractGame GetGame(ProcessGameRequest request)
		{
			return GameFactory.CreateGameFactory(request.Type, request.BetAmount);
		}

		public AbstractGame RestoreGame(GameData request)
		{
			return GameFactory.RestoreGameFactory(request);
		}

		public async Task CreateGameAction(ProcessGameRequest request, Guid gameSessionId)
		{
			await _gameRepository.CreateGameAction(request, gameSessionId);
		}

		public async Task<List<Games>> GetGames()
		{
			return await _gameRepository.GetGames();
		}

		public async Task<List<Games>> GetGamesByCategory(int category)
		{
			GameCategory gameCategory = (GameCategory)category;
			return await _gameRepository.GetGamesByCategory(gameCategory);
		}

		public async Task<bool> CheckIfGameEnded(Guid gameSessionId)
		{
			return await _gameRepository.CheckIfGameEnded(gameSessionId);
		}

		public async Task<Games> CreateGame(CreateGameRequest createGame)
		{
			return await _gameRepository.CreateGame(createGame);
		}

		public async Task<Guid> GetSessionForUser(UserSessionRequest request)
		{
			return await _gameRepository.GetSessionForUser(request);
		}

		public async Task CreateGameHistoryRecord(GameSession gameSession)
		{
			await _gameRepository.CreateGameHistoryRecord(gameSession);
		}

		public async Task UpdateIsGameActive(AdminGameChangeActiveRequest request)
		{
			await _gameRepository.UpdateIsGameActive(request);
		}

		public GameActionResponse MakeMove(AbstractGame game, Dictionary<string, object> data)
		{
			switch (game)
			{
				case PlinkoGame plinko:
					{
						plinko.CalculateDrop();
						var dict = new Dictionary<string, object>();
						dict["Position"] = plinko.GetBallPosition();
						dict["Path"] = plinko.GetPath();
						return new GameActionResponse
						{
							Success = true,
							Message = new GameEnded
							{
								Status = plinko.GetStatus(),
								Multiplier = plinko.GetMultiplier(),
								Result = plinko.GetWinnedAmount(),
								Data = dict
							}

						};
					}
				case MinesGame mines:
					{
						var X = JsonSerializer.Deserialize<int>(data["X"].ToString());
						var Y = JsonSerializer.Deserialize<int>(data["Y"].ToString());

						MinesPosition movePosition = new MinesPosition { X = X, Y = Y };
						var isOver = mines.ValidateMove(movePosition);
						if (isOver) {
							var dict = new Dictionary<string, object>();
							dict["Fields"] = mines.GetField();
							return new GameActionResponse
							{
								Success = true,
								Message = new GameEnded
								{
									Status = mines.GetStatus(),
									Data = dict
								}
							};
						}
						return new GameActionResponse
						{
							Success = true,
							Message = new GameEnded
							{
								Status = mines.GetStatus(),
								Multiplier = mines.GetMultiplier(),
								Result = mines.GetWinnedAmount(),
							}
						};
					}
				case FrogGame frog:
					{
						var isGameOver = frog.IsGameOver();
						var dict = new Dictionary<string, object>();
						dict["GameOver"] = isGameOver;
						return new GameActionResponse
						{
							Success = true,
							Message = new GameEnded
							{
								Status = frog.GetStatus(),
								Multiplier = frog.GetMultiplier(),
								Result = frog.GetWinnedAmount(),
								Data = dict
							}
						};
					}
				case DiceGame dice:
					{
						dice.ValidateMove();
						var dict = new Dictionary<string, object>();
						dict["DiceSum"] = dice.GetDiceSum();
						var dices = dice.GetDices();
						dict["Dices"] = dices;
						return new GameActionResponse
						{
							Success = true,
							Message = new GameEnded
							{
								Status = dice.GetStatus(),
								Multiplier = dice.GetMultiplier(),
								Result = dice.GetWinnedAmount(),
								Data = dict
							}
						};
					}
				case BlackJackGame blackJack:
					{
						var playerMove = JsonSerializer.Deserialize<PlayerMove>(data["PlayerMove"].ToString());
						var dict = new Dictionary<string, object>();
						switch (playerMove)
						{
							case PlayerMove.Hit:
								{
									blackJack.HitPlayer();
									break;
								}
							case PlayerMove.Stand:
								{
									blackJack.Stand();
									dict["PlayerCards"] = blackJack.GetPlayerHand();
									dict["DealerCards"] = blackJack.GetDealerHand();
									dict["PlayerSum"] = blackJack.GetPlayerSum();
									dict["DealerSum"] = blackJack.GetDealerSumWithHidden();
									dict["Result"] = blackJack.GetCashWon();
									return new GameActionResponse
									{
										Success = true,
										Message = new GameEnded
										{
											Status = blackJack.GetStatus(),
											Multiplier = blackJack.GetMultiplier(),
											Result = blackJack.GetWinnedAmount(),
											Data = dict
										}
									};
								}
							case PlayerMove.Info:
								{
									dict["PlayerCards"] = blackJack.GetPlayerHand();
									dict["DealerCards"] = blackJack.GetDealerHand();
									dict["PlayerSum"] = blackJack.GetPlayerSum();
									dict["DealerSum"] = blackJack.GetDealerSum();	
									return new GameActionResponse
									{
										Success = true,
										Message = new GameEnded
										{
											Status = blackJack.GetStatus(),
											Multiplier = blackJack.GetMultiplier(),
											Data = dict
										}
									};
								}
						}
						dict["PlayerCards"] = blackJack.GetPlayerHand();
						dict["DealerCards"] = blackJack.GetDealerHand();
						dict["PlayerSum"] = blackJack.GetPlayerSum();
						dict["DealerSum"] = blackJack.GetDealerSum();
						return new GameActionResponse
						{
							Success = true,
							Message = new GameEnded
							{
								Status = blackJack.GetStatus(),
								Multiplier = blackJack.GetMultiplier(),
								Result = blackJack.GetWinnedAmount(),
								Data = dict
							}
						};
					}
				default: return null;

			}
		}

		public GameActionResponse CashOutEarly(AbstractGame game, Dictionary<string, object> data)
		{
			switch (game)
			{
				case MinesGame mines:
					{
						mines.CashOut();
						var dict = new Dictionary<string, object>();
						dict["Fields"] = mines.GetField();
						dict["Result"] = mines.GetCashWon();
						return new GameActionResponse
						{
							Success = true,
							Message = new GameEnded
							{
								Status = mines.GetStatus(),
								Multiplier = mines.GetMultiplier(),
								Data = dict
							}
						};
					}
				case FrogGame frog:
					{
						frog.CashOut();
						var dict = new Dictionary<string, object>();
						dict["Result"] = frog.GetCashWon();
						return new GameActionResponse
						{
							Success = true,
							Message = new GameEnded
							{
								Status = frog.GetStatus(),
								Multiplier = frog.GetMultiplier(),
								Data = dict
							}
						};

					}
				default: return null;
			}
		}

		public async Task SaveSession(GameSession gameSession)
		{
			await _gameRepository.SaveSession(gameSession);
		}


	}
}

