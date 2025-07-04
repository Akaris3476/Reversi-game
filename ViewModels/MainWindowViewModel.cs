using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Reversi_game.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting => "Reversi";
    public int GridRows{ get; } = 8;
    public int GridSizePx { get; } = 600;

    public readonly List<Point> Center4Tiles;
    
    public int TurnCount = 0;
        
    private bool _turn; //false == black, true == white
    
    
    public bool Turn
    {
        get => _turn;
        set
        {
            _turn = value;
		    TileList[0].UpdateGridAvailable();
            
            
            if (EnemyBotEnabled && _enemyTurn == _turn && _gameOver == false)
            {

                switch (_botType)
                {
                    case BotTypeEnum.Minimax:
                        Console.WriteLine("Bot: minimax");
                        EnemyMinmaxTurn();
                        break;
                    case BotTypeEnum.Random:
                        Console.WriteLine("Bot: random");
                        EnemyRandomTurn();
                        break;
                }
                
            }
        }
    }
    

    
    
    private bool _gameOver;
    
    public Tile.ColorEnum Winner = Tile.ColorEnum.Blank;
    
    private int _finalWhiteTilesCount;
    private int _finalBlackTilesCount;

    public void InitiateGameEnd()
    {
        _gameOver = true;
        Console.WriteLine("END GAME!!");
			
        _finalBlackTilesCount = 0;
        _finalWhiteTilesCount = 0;
		
        foreach (var tile in TileList)
        {
				
            if (tile.TileColor == Tile.ColorEnum.Black)
                _finalBlackTilesCount++;
				
            if (tile.TileColor == Tile.ColorEnum.White)
                _finalWhiteTilesCount++;
				
        }
        
        SetGameResults();
    }
    private void SetGameResults()
    {
        if (_finalBlackTilesCount > _finalWhiteTilesCount)
        {
            Winner = Tile.ColorEnum.Black;
        }
        else
        {
            Winner = Tile.ColorEnum.White;
        }
        UpdateTurnString();
    }
    public string TurnString => 
        _gameOver ?  $"Game Over. Winner: {Winner.ToString()}\nPoints: Black {_finalBlackTilesCount}, White {_finalWhiteTilesCount}" 
            : (Turn ? "Turn: White" : "Turn: Black");

    public ObservableCollection<Tile> TileList { get; set; }

    public void UpdateTurnString()
    {
        OnPropertyChanged(nameof(TurnString));
    }
    public MainWindowViewModel()
    {




        Point upperLeft = new(GridRows/2 - 1 , GridRows/2 - 1);
        Point upperRight = new(GridRows/2, GridRows/2 - 1);
        Point downLeft = new(GridRows/2 - 1, GridRows/2);
        Point downRight = new(GridRows/2, GridRows/2);

        Center4Tiles = new List<Point>
        {
            upperLeft, upperRight,
            downLeft, downRight
        };

        
        TileList = new ObservableCollection<Tile>();

        for (int y = 0; y < GridRows; y++)
        {
            for (int x = 0; x < GridRows; x++)
            {
                TileList.Add(new Tile(x,y, this));
            }
        }
        
        MediumDifficulty = true;
        
        TileList[0].UpdateGridAvailable();


    }


    public MainWindowViewModel(MainWindowViewModel window)
    {

        EnemyBotEnabled = false;
        Center4Tiles = new List<Point>(window.Center4Tiles);
        
        TileList = new ObservableCollection<Tile>();

        foreach (var tile in window.TileList)
        {
            TileList.Add(new Tile(tile, this));
        }
        
        
        TurnCount = window.TurnCount;
        _botType = window._botType;
        Turn = window.Turn;

    }


}