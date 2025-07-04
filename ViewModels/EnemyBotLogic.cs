using System;
using System.Collections.Generic;
using System.Linq;

namespace Reversi_game.ViewModels;

public partial class MainWindowViewModel
{
	
	private bool _enemyBotEnabled = true;
    private bool _enemyTurn = true;
	
    public bool EnemyBotEnabled
	{
		get => _enemyBotEnabled;
		set
		{
			_enemyTurn = !_turn;
			_enemyBotEnabled = value;
		}
        
	}
    
    
    
	public enum BotTypeEnum { Random, Minimax }
	private BotTypeEnum _botType = BotTypeEnum.Minimax;

	public bool RandomBot
	{
		set
		{
			_botType = BotTypeEnum.Random;
		}
	}
	
	
	public enum DifficultyEnum { Easy, Medium, Hard }
	private DifficultyEnum _difficulty = DifficultyEnum.Medium;
	
	public bool EasyDifficulty
	{
		set
		{
			_difficulty = DifficultyEnum.Easy;
			_botType = BotTypeEnum.Minimax;
		}
	} 
	public bool MediumDifficulty
	{
		set
		{
			_difficulty = DifficultyEnum.Medium;
			_botType = BotTypeEnum.Minimax;
		}
	} 
	public bool HardDifficulty
	{
		set
		{
			_difficulty = DifficultyEnum.Hard;
			_botType = BotTypeEnum.Minimax;
		}
	} 
	
	private void EnemyRandomTurn()
	{
        
		int randomAvailableTileIndex = RandomAvailableTileIndex();
		if (randomAvailableTileIndex == -1) return;
        
		TileList[randomAvailableTileIndex].Tile_Click();
	}

	private int RandomAvailableTileIndex()
	{
        
		List<Tile> availableTiles = GetAvailableTiles();
        
		if (availableTiles.Count == 0) return -1;
        

		Random rnd = new();
		int randomIndex = rnd.Next(availableTiles.Count);

		return TileList.IndexOf(availableTiles[randomIndex]);

	}

	private List<Tile> GetAvailableTiles()
	{
		List<Tile> availableTiles = new();
        
        
		foreach (var tile in  TileList)
		{
			if (tile.TileColor != Tile.ColorEnum.Available) continue;
            
			availableTiles.Add(tile);
		}
        
		return availableTiles;
	}
	
	
	private void EnemyMinmaxTurn()
	{
		List<Tile> availableTilesList = GetAvailableTiles();
		List<(Tile Tile, int Value)> turnsValuesList = new();
                        
		foreach (var tile in  availableTilesList)
		{
			var newPos = new MainWindowViewModel(this);
			newPos.TileList[TileList.IndexOf(tile)].Tile_Click();
                            
			int turnValue = Minimax(newPos, 2, Int32.MinValue, Int32.MaxValue, _enemyTurn);

			Console.Write(turnValue.ToString() + ", ");
			turnsValuesList.Add((tile, turnValue));
                            
		}

		Console.WriteLine();
                        
		turnsValuesList.Sort((a, b) => a.Value.CompareTo(b.Value));

		if (_enemyTurn)
		{
			Console.WriteLine("Enemy Color: white");
			switch (_difficulty)
			{
				case DifficultyEnum.Easy:
					Console.WriteLine("Difficulty: easy");
					turnsValuesList.First().Tile.Tile_Click();
					break;
				case DifficultyEnum.Medium:
					Console.WriteLine("Difficulty: medium");
					int index = turnsValuesList.Count() / 2;
					turnsValuesList[index].Tile.Tile_Click();	
					break;
				case DifficultyEnum.Hard:
					Console.WriteLine("Difficulty: hard");
					turnsValuesList.Last().Tile.Tile_Click();
					break;
			}
		}
		else
		{
			Console.WriteLine("Enemy Color: black");
			switch (_difficulty)
			{
				case DifficultyEnum.Easy:
					Console.WriteLine("Difficulty: easy");
					turnsValuesList.Last().Tile.Tile_Click();
					break;
				case DifficultyEnum.Medium:
					Console.WriteLine("Difficulty: medium");
					int index = turnsValuesList.Count() / 2;
					turnsValuesList[index].Tile.Tile_Click();	
					break;
				case DifficultyEnum.Hard:
					Console.WriteLine("Difficulty: hard");
					turnsValuesList.First().Tile.Tile_Click();
					break;
			}
		}

                        
		Console.WriteLine("Number of minimax iterations: {0}", _minimaxIterations);
		_minimaxIterations = 0;
	}
	
	private int _minimaxIterations;
	
	
	public int Minimax(MainWindowViewModel position, int depth, int alpha, int beta, bool maximizingPlayer)
	{
		_minimaxIterations++;
	
		if (depth == 0 || position._gameOver)
		{
			int value = EvaluateGameState(position.TileList);
			return value;
		}
		
		if (maximizingPlayer)
		{

			int maxEval = Int32.MinValue;

			foreach (var tile in position.TileList)
			{
				if (tile.TileColor != Tile.ColorEnum.Available) continue;
				
				var newPos = new MainWindowViewModel(position);
				newPos.TileList[position.TileList.IndexOf(tile)].Tile_Click();
				
				int eval = Minimax(newPos, depth - 1, alpha, beta, false);
				maxEval = Math.Max(maxEval, eval);
				alpha = Math.Max(alpha, eval);
				if (beta <= alpha) break;
			}

			return maxEval;
			
		}
		else
		{
			
			int minEval = Int32.MaxValue;
			
			foreach (var tile in position.TileList)
			{
				if (tile.TileColor != Tile.ColorEnum.Available) continue;
				
				var newPos = new MainWindowViewModel(position);
				newPos.TileList[position.TileList.IndexOf(tile)].Tile_Click();
				
				int eval = Minimax(newPos, depth - 1, alpha, beta, true);
				minEval = Math.Min(minEval, eval);
				beta = Math.Min(beta, eval);
				if (beta <= alpha) break;
			}

			return minEval;
		}
        
	}
	
	//more value = good position for white
	//less value = good position for black
	private int EvaluateGameState(IList<Tile> tileList)
	{
		
		int blackTilesCount = 0;
		int whiteTilesCount = 0;
		
		foreach (var tile in tileList)
		{
			if (tile.TileColor == Tile.ColorEnum.Black) {blackTilesCount++; continue; }
			if (tile.TileColor == Tile.ColorEnum.White) whiteTilesCount++;
		}
		
		return whiteTilesCount - blackTilesCount;
		


	}
	
}