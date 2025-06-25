using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Reversi_game.ViewModels;

public partial class Tile : ViewModelBase
{
	public Point Coordinate { get; init; }
	
	//ref to parent window
	private MainWindowViewModel _parentWindow;

	public int TileSize { get; set; }
	
	public Tile(int x, int y,MainWindowViewModel parentWindow )
	{
		Coordinate = new Point(x,y);
		_parentWindow = parentWindow;
		TileSize = _parentWindow.GridSizePx / _parentWindow.GridRows ;
	}
	
	public enum ColorEnum { Black, White, Blank, Available }
	
	[ObservableProperty]
	private ColorEnum _tileColor = ColorEnum.Blank;
	partial void OnTileColorChanged(ColorEnum oldValue, ColorEnum newValue)
	{
		OnPropertyChanged(nameof(ColorName));
		
	}
	
	// returns string of TileColor
	public string ColorName
	{
		get
		{
			switch (TileColor)
			{
				case ColorEnum.Available:
					return "ForestGreen";
				case ColorEnum.Blank:
					return "Transparent";
				default:
					return TileColor.ToString();
			}
		}
	}
	
	public void Tile_Click()
	{
		//return if tile is not available
		if (TileColor != ColorEnum.Available) return;
		

		if (_parentWindow.Turn)
		{
			TileColor = ColorEnum.White;
			
		}
		else
		{
			TileColor = ColorEnum.Black;
			
		}
		_parentWindow.TurnCount++;
		
		if (_directions.Count > 0) //flip line if possible
		{
			foreach (var direction in _directions)
			{
				
				Console.WriteLine( $"Turn: {_parentWindow.TurnCount.ToString()}\n" + $"Coordinate: {Coordinate.ToString()}\n" +direction.ToString());
				
				ReverseLine(direction);
			}
		}
		
		
		_parentWindow.Turn = !_parentWindow.Turn;
		_parentWindow.UpdateTurnString();
		
		

	}
	
	
	//says if the tile is in the center of the grid
	private bool InCenter()	
	{
		foreach (var centerTile in _parentWindow.Center4Tiles)
		{
			if (Coordinate == centerTile)
			{
				return true;
			}
		}
		return false;
	}

	
	//update grid with green available tiles
	public void UpdateGridAvailable()
	{
		var tileList = _parentWindow.TileList;
		bool isThereGreenTile = false;
		
		
		foreach (var tile in tileList)
		{
			//for first 4 turns draw center tiles green
			if (_parentWindow.TurnCount < 4)
			{
				isThereGreenTile = true;
				if ( tile.TileColor == ColorEnum.Blank && tile.InCenter()) 
				{
					tile.TileColor = ColorEnum.Available;
				}
				continue;
			}


			//draw available tiles for another part of the game
			if (tile.TileColor == ColorEnum.Available) tile.TileColor = ColorEnum.Blank; //reset green from last turn
			
			
			if (tile.TileColor == ColorEnum.Blank && tile.CanBeFliped(tile.Coordinate) )
			{
				tile.TileColor = ColorEnum.Available;
				isThereGreenTile = true;
			}
			
			
		}

		if (!isThereGreenTile)
		{
			Console.WriteLine("END GAME!!");
			
			int blackTilesCount = 0;
			int whiteTilesCount = 0;
		
			foreach (var tile in tileList)
			{
				if (tile.TileColor == ColorEnum.Black)
				{
					blackTilesCount++;
				} else if (tile.TileColor == ColorEnum.White)
				{
					whiteTilesCount++;
				}
			}

			//send players results and end the game
			_parentWindow.SetGameResults(whiteTilesCount, blackTilesCount);
		}
	}


	private enum DirectionsEnum { Up, Down, Left, Right, UpperLeft, UpperRight, LowerLeft, LowerRight }
	private List<DirectionsEnum> _directions = new();
	
	//checks possibility of a turn on specific tile so it can be painted green
	private bool CanBeFliped(Point coordinate) 
	{
		var tileList = _parentWindow.TileList;
		var gridRows = _parentWindow.GridRows;
		
		bool enemyTileOnTheWay = false;
		
		var enemyTile = _parentWindow.Turn ? ColorEnum.Black : ColorEnum.White; 
		var friendlyTile = _parentWindow.Turn ? ColorEnum.White : ColorEnum.Black; 
		
		_directions = new();
		
		//each for loop beams point in different direction and checks for enemy plus friendly tiles on the way
		//so at least one enemy tile can be reversed
		
		//up and right
		for (Point p = new(coordinate.X + 1, coordinate.Y - 1 );
		     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X + 1, p.Y - 1))
		{
			Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
			
			// stop if there is already available tile
			if (pTile.TileColor == ColorEnum.Available || pTile.TileColor == ColorEnum.Blank) break;
			//mark if there enemy tile on the way
			if (pTile.TileColor == enemyTile) { enemyTileOnTheWay = true; continue; } 
			//there is friendly tile on the way but no enemy
			if (pTile.TileColor == friendlyTile && !enemyTileOnTheWay) break;
			//friendly tile after enemy
			if (enemyTileOnTheWay && pTile.TileColor == friendlyTile) _directions.Add(DirectionsEnum.UpperRight);
		}

		enemyTileOnTheWay = false;
		//up and left
		for (Point p = new(coordinate.X - 1, coordinate.Y - 1);
		     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X - 1, p.Y - 1))
		{
			Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
			
			// stop if there is already available tile
			if (pTile.TileColor == ColorEnum.Available || pTile.TileColor == ColorEnum.Blank) break;
			//mark if there enemy tile on the way
			if (pTile.TileColor == enemyTile) { enemyTileOnTheWay = true; continue; } 
			//there is friendly tile on the way but no enemy
			if (pTile.TileColor == friendlyTile && !enemyTileOnTheWay) break;
			//friendly tile after enemy
			if (enemyTileOnTheWay && pTile.TileColor == friendlyTile) _directions.Add(DirectionsEnum.UpperLeft);
		}

		enemyTileOnTheWay = false;
		//right
		for (Point p = new(coordinate.X + 1, coordinate.Y);
		     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X + 1, p.Y))
		{
			Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
			
			// stop if there is already available tile
			if (pTile.TileColor == ColorEnum.Available || pTile.TileColor == ColorEnum.Blank) break;
			//mark if there enemy tile on the way
			if (pTile.TileColor == enemyTile) { enemyTileOnTheWay = true; continue; } 
			//there is friendly tile on the way but no enemy
			if (pTile.TileColor == friendlyTile && !enemyTileOnTheWay) break;
			//friendly tile after enemy
			if (enemyTileOnTheWay && pTile.TileColor == friendlyTile) _directions.Add(DirectionsEnum.Right);;
		}
		enemyTileOnTheWay = false;
		//left
		for (Point p = new(coordinate.X - 1, coordinate.Y);
		     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X - 1, p.Y))
		{
			Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
			
			// stop if there is already available tile
			if (pTile.TileColor == ColorEnum.Available || pTile.TileColor == ColorEnum.Blank) break;
			//mark if there enemy tile on the way
			if (pTile.TileColor == enemyTile) { enemyTileOnTheWay = true; continue; } 
			//there is friendly tile on the way but no enemy
			if (pTile.TileColor == friendlyTile && !enemyTileOnTheWay) break;
			//friendly tile after enemy
			if (enemyTileOnTheWay && pTile.TileColor == friendlyTile) _directions.Add(DirectionsEnum.Left);
		}
		
		enemyTileOnTheWay = false;
		//up
		for (Point p = new(coordinate.X, coordinate.Y - 1);
		     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X, p.Y - 1 ))
		{
			Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
			
			// stop if there is already available tile
			if (pTile.TileColor == ColorEnum.Available || pTile.TileColor == ColorEnum.Blank) break;
			//mark if there enemy tile on the way
			if (pTile.TileColor == enemyTile) { enemyTileOnTheWay = true; continue; } 
			//there is friendly tile on the way but no enemy
			if (pTile.TileColor == friendlyTile && !enemyTileOnTheWay) break;
			//friendly tile after enemy
			if (enemyTileOnTheWay && pTile.TileColor == friendlyTile) _directions.Add(DirectionsEnum.Up);
		}
		
		enemyTileOnTheWay = false;
		//down
		for (Point p = new(coordinate.X, coordinate.Y + 1);
		     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X, p.Y + 1 ))
		{
			Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
			
			// stop if there is already available tile
			if (pTile.TileColor == ColorEnum.Available || pTile.TileColor == ColorEnum.Blank) break;
			//mark if there enemy tile on the way
			if (pTile.TileColor == enemyTile) { enemyTileOnTheWay = true; continue; } 
			//there is friendly tile on the way but no enemy
			if (pTile.TileColor == friendlyTile && !enemyTileOnTheWay) break;
			//friendly tile after enemy
			if (enemyTileOnTheWay && pTile.TileColor == friendlyTile) _directions.Add(DirectionsEnum.Down);
		}

		enemyTileOnTheWay = false;
		//down and left
		for (Point p = new(coordinate.X - 1, coordinate.Y + 1);
		     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X - 1, p.Y + 1 ))
		{
			Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
			
			// stop if there is already available tile
			if (pTile.TileColor == ColorEnum.Available || pTile.TileColor == ColorEnum.Blank) break;
			//mark if there enemy tile on the way
			if (pTile.TileColor == enemyTile) { enemyTileOnTheWay = true; continue; } 
			//there is friendly tile on the way but no enemy
			if (pTile.TileColor == friendlyTile && !enemyTileOnTheWay) break;
			//friendly tile after enemy
			if (enemyTileOnTheWay && pTile.TileColor == friendlyTile) _directions.Add(DirectionsEnum.LowerLeft);
		}
		
		enemyTileOnTheWay = false;
		//down and right
		for (Point p = new(coordinate.X + 1, coordinate.Y +1);
		     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X + 1, p.Y + 1 ))
		{
			Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
			
			// stop if there is already available tile
			if (pTile.TileColor == ColorEnum.Available || pTile.TileColor == ColorEnum.Blank) break;
			//mark if there enemy tile on the way
			if (pTile.TileColor == enemyTile) { enemyTileOnTheWay = true; continue; } 
			//there is friendly tile on the way but no enemy
			if (pTile.TileColor == friendlyTile && !enemyTileOnTheWay) break;
			//friendly tile after enemy
			if (enemyTileOnTheWay && pTile.TileColor == friendlyTile) _directions.Add(DirectionsEnum.LowerRight);
		}
		
		


		

		return _directions.Count > 0 ;
	}

	private void ReverseLine(DirectionsEnum direction)
	{
		
		var gridRows = _parentWindow.GridRows;
		var tileList = _parentWindow.TileList;
		
		var enemyTile = _parentWindow.Turn ? ColorEnum.Black : ColorEnum.White;
		var friendlyTile = _parentWindow.Turn ? ColorEnum.White : ColorEnum.Black; 
		
		
		switch (direction)
		{
			case DirectionsEnum.Down:
				for (Point p = new(Coordinate.X, Coordinate.Y + 1);
				     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X, p.Y + 1 ))
				{
					Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
					
					//mark if there enemy tile on the way
					if (pTile.TileColor == enemyTile)
					{
						pTile.TileColor = friendlyTile;
						continue;
					} 
					break;

				}

				break;
			case DirectionsEnum.Up:
				for (Point p = new(Coordinate.X, Coordinate.Y - 1);
				     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X, p.Y - 1 ))
				{
					Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
					
					//mark if there enemy tile on the way
					if (pTile.TileColor == enemyTile)
					{
						pTile.TileColor = friendlyTile;
						continue;
					} 
					break;

				}
				break;
			case DirectionsEnum.Left:
				for (Point p = new(Coordinate.X - 1, Coordinate.Y );
				     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X - 1, p.Y ))
				{
					Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
					
					//mark if there enemy tile on the way
					if (pTile.TileColor == enemyTile)
					{
						pTile.TileColor = friendlyTile;
						continue;
					} 
					break;

				}
				break;
			case DirectionsEnum.Right:
				for (Point p = new(Coordinate.X + 1, Coordinate.Y );
				     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X + 1, p.Y ))
				{
					Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
					
					//mark if there enemy tile on the way
					if (pTile.TileColor == enemyTile)
					{
						pTile.TileColor = friendlyTile;
						continue;
					} 
					break;

				}
				break;
			case DirectionsEnum.UpperLeft:
				for (Point p = new(Coordinate.X - 1, Coordinate.Y - 1);
				     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X -1 , p.Y - 1 ))
				{
					Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
					
					//mark if there enemy tile on the way
					if (pTile.TileColor == enemyTile)
					{
						pTile.TileColor = friendlyTile;
						continue;
					} 
					break;

				}
				break;
			case DirectionsEnum.UpperRight:
				for (Point p = new(Coordinate.X + 1, Coordinate.Y - 1);
				     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X + 1, p.Y - 1 ))
				{
					Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
					
					//mark if there enemy tile on the way
					if (pTile.TileColor == enemyTile)
					{
						pTile.TileColor = friendlyTile;
						continue;
					} 
					break;

				}
				break;
			case DirectionsEnum.LowerLeft:
				for (Point p = new(Coordinate.X - 1, Coordinate.Y + 1);
				     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X - 1, p.Y + 1 ))
				{
					Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
					
					//mark if there enemy tile on the way
					if (pTile.TileColor == enemyTile)
					{
						pTile.TileColor = friendlyTile;
						continue;
					} 
					break;

				}
				break;
			case DirectionsEnum.LowerRight:
				for (Point p = new(Coordinate.X + 1, Coordinate.Y + 1);
				     (p.X < gridRows && p.X >= 0) && (p.Y < gridRows && p.Y >= 0); p = new Point(p.X  +1, p.Y + 1 ))
				{
					Tile pTile = tileList.FirstOrDefault(t => t.Coordinate == p);
					
					//mark if there enemy tile on the way
					if (pTile.TileColor == enemyTile)
					{
						pTile.TileColor = friendlyTile;
						continue;
					} 
					break;

				}
				break;
			
		}
		
	}
}



