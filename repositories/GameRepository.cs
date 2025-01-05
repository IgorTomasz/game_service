using game_service.classes;
using game_service.context;
using game_service.models;
using game_service.models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace game_service.repositories
{
	public interface IGameRepository
	{
		public Task<Guid> CreateGameSession(AbstractGame game, Guid userId, Guid userSessionId);
		public Task CreateGameAction(ProcessGameRequest request, Guid gameSessionId);
		public Task<GameSession> GetGameSession(Guid gameSessionId);
		public Task SaveSession(GameSession gameSession);
		public Task<List<Games>> GetGames();
		public Task<List<Games>> GetGamesByCategory(GameCategory category);
		public Task<Games> CreateGame(CreateGameRequest createGame);
		public Task<bool> CheckIfGameEnded(Guid gameSessionId);
		public Task<Guid> GetSessionForUser(UserSessionRequest request);
		public Task<Guid> CreateGameHistoryRecord(GameSession gameSession);
		public Task UpdateIsGameActive(AdminGameChangeActiveRequest request);
		public Task<List<GameSession>> EndAllActiveSessions(UserSessionRequest request);
		public Task<List<Games>> GetGamesAdmin();
	}

	public class GameRepository : IGameRepository
	{
		private readonly GameDatabaseContext _context;

		public GameRepository(GameDatabaseContext context)
		{
			_context = context;

		}

		public async Task<Guid> CreateGameSession(AbstractGame game, Guid userId, Guid userSessionId)
		{
			Guid guid = Guid.NewGuid();
			GameSession gameSession = new GameSession
			{
				GameSessionId = guid,
				UserId = userId,
				UserSessionId = userSessionId,
				GameHistoryId = null,
				BetAmount = game.BetAmount,
				GameType = game.Type,
				Result = null,
				CashWon = game.GetWinnedAmount(),
				StartTime = DateTime.UtcNow.AddHours(1),
				EndTime = null,
				Status = game.GetStatus(),
				CurrentMultiplier = game.GetMultiplier(),
				CashedOutEarly = game.CashOutEarly,
				Game = game,
			};

			await _context.GameSessions.AddAsync(gameSession);
			await _context.SaveChangesAsync();
			return gameSession.GameSessionId;
		}

		public async Task<List<GameSession>> EndAllActiveSessions(UserSessionRequest request)
		{
			var sessions = await _context.GameSessions.Where(x => x.UserId == request.UserId && x.UserSessionId == request.UserSessionId && x.EndTime == null).ToListAsync();
			sessions.ForEach(e=>
			{
				e.EndTime = DateTime.UtcNow.AddHours(1);
				e.Status = GameStatus.EndedLose;
			});
			await _context.SaveChangesAsync();

			return sessions;
		}

		public async Task CreateGameAction(ProcessGameRequest request, Guid gameSessionId)
		{
			GameAction action = new GameAction
			{
				GameActionId = Guid.NewGuid(),
				GameSessionId = gameSessionId,
				Timestamp = DateTime.UtcNow.AddHours(1),
				Type = request.Action,
				ActionData = request.Data
			};

			await _context.GameActions.AddAsync(action);
			await _context.SaveChangesAsync();


		}

		public async Task<GameSession> GetGameSession(Guid gameSessionId)
		{
			return await _context.GameSessions.FindAsync(gameSessionId);
		}

		public async Task SaveSession(GameSession gameSession)
		{
			var sess = await _context.GameSessions.FindAsync(gameSession.GameSessionId);

			if(sess.Status == GameStatus.EndedWin || sess.Status == GameStatus.EndedLose)
			{
				sess.EndTime = DateTime.UtcNow.AddHours(1);
				sess.CashWon = gameSession.Game.GetWinnedAmount();
			}

			sess.Game = gameSession.Game;
			await _context.SaveChangesAsync();
		}

		public async Task<List<Games>> GetGames()
		{
			return await _context.Games.Where(x => x.IsActive==true).ToListAsync();
		}

		public async Task<List<Games>> GetGamesAdmin()
		{
			return await _context.Games.ToListAsync();
		}

		public async Task<List<Games>> GetGamesByCategory(GameCategory category)
		{
			return await _context.Games.Where(x => x.IsActive == true && x.GameCategory==category).ToListAsync();
		}

		public async Task<bool> CheckIfGameEnded(Guid gameSessionId)
		{
			var gameSession = await _context.GameSessions.Where(x => x.GameSessionId==gameSessionId).FirstOrDefaultAsync();
			if (gameSession != null)
			{
				return gameSession.Status == GameStatus.EndedLose || gameSession.Status == GameStatus.EndedWin ? true : false;
			}
			
			return false;

		}

		public async Task<Guid> CreateGameHistoryRecord(GameSession gameSession)
		{
			Guid guid = Guid.NewGuid();
			GameHistory gameHistory = new GameHistory
			{
				GameHistoryId = guid,
				GameSessionId = gameSession.GameSessionId,
				UserId = gameSession.UserId,
				MaxMultiplier = gameSession.CurrentMultiplier,
				Result = gameSession.Status == GameStatus.EndedLose ? 0 : gameSession.CashWon,
				BetAmount = gameSession.BetAmount,
				Timestamp = DateTime.UtcNow.AddHours(1)
			};
			await _context.GameHistory.AddAsync(gameHistory);
			
			await _context.SaveChangesAsync();

			return guid;
		}

		public async Task<Guid> GetSessionForUser(UserSessionRequest request)
		{
			var session = await _context.GameSessions.Where(x => x.UserId == request.UserId && x.UserSessionId == request.UserSessionId && x.GameType == request.GameType && x.EndTime == null).FirstOrDefaultAsync();
			return session.GameSessionId;
		}

		public async Task<Games> CreateGame(CreateGameRequest createGame)
		{
			var game = new Games
			{
				GameName = createGame.GameName,
				GameAdditionalFields = createGame.GameAdditionalFields,
				GameDescription = createGame.GameDescription,
				GameCategory = createGame.GameCategory,
				IsActive = createGame.IsActive,
			};

			await _context.Games.AddAsync(game);
			await _context.SaveChangesAsync();
			return game;
		}

		public async Task UpdateIsGameActive(AdminGameChangeActiveRequest request)
		{
			foreach (var item in request.gameActives)
			{
				var game = await _context.Games.FindAsync(item.GameId);
				game.IsActive = item.IsActive;
				await _context.SaveChangesAsync();
			}
		}
	}
}
