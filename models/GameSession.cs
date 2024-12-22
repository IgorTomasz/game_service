using game_service.classes;
using game_service.classes.games;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace game_service.models
{
	public class GameSession
	{
		[Key]
		public Guid GameSessionId { get; set; }
		[Required]
		public Guid UserId { get; set; }
		[Required] 
		public Guid UserSessionId { get; set; }
		public Guid? GameHistoryId { get; set; }
		[Required]
		public decimal BetAmount { get; set; }
		[Required]
		public GameType GameType { get; set; }
		public ResultType? Result {  get; set; }
		[Required]
		public decimal CashWon {  get; set; }
		[Required]
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		[Required]
		public GameStatus Status { get; set; }
		[Required]
		public decimal CurrentMultiplier { get; set; }
		[Required]
		public bool CashedOutEarly { get; set; }
		public virtual ICollection<GameAction> GameActions { get; set; }
		[ForeignKey(nameof(GameHistoryId))]
		public virtual GameHistory History { get; set; }
		[Required]
		public string SerializedGame {  get; set; }
		[NotMapped]
		public AbstractGame Game { get => DeserializeGameData(); set=> SerializedGame = SerializeGame(value); }

		private string SerializeGame(AbstractGame game)
		{
			var data = new GameData
			{
				GameType = this.GameType,
				BetAmount = game.BetAmount,
				CurrentMultiplier = game.CurrentMultiplier,
				Status = game.Status,
				GamesValues = GetGameValues(game),
			};

			return JsonSerializer.Serialize(data);
		}

		private AbstractGame DeserializeGameData()
		{

			var game = JsonSerializer.Deserialize<GameData>(SerializedGame);
			return GameFactory.RestoreGameFactory(game);
		}

		public Dictionary<string, object> GetGameValues(AbstractGame game)
		{
			var data = new Dictionary<string, object>();

			switch (game)
			{
				case MinesGame mines:
					data["Field"] = mines.Field;
					data["MinesCount"] = mines.MinesCount;
					data["DiscoveredDiamonds"] = mines.DiscoveredDiamonds; break;
				case DiceGame dice:
					break;
				case PlinkoGame plinko:
					data["Rows"] = plinko.RowsCount;
					data["Difficulty"] = plinko.ChoosenDifficulty;
					data["FinalBallPosition"] = plinko.FinalBallPosition;
					data["PlinkoPositions"] = plinko.PlinkoPositions;
					data["Path"] = plinko.Path; break;
				case ChickenGame chicken:
					break;
				case BlackJackGame blackJack:
					break;
			}

			return data;
		}
	}
	public enum ResultType
	{
		Won, Lost
	}
}
