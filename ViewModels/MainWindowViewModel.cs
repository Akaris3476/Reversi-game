using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;


namespace Reversi_game.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting => "Reversi";
    public int GridRows{ get; } = 8;
    public int GridSizePx { get; } = 600;

    public List<Point> Center4Tiles;
    
    public int TurnCount = 0;
        
    private bool _turn; //false == black, true == white

    private bool _enemyBotEnabled = true;
    public bool EnemyBotEnabled
    {
        get => _enemyBotEnabled;
        set
        {
            _enemyTurn = !_turn;
            _enemyBotEnabled = value;
        }
        
    }
    private bool _enemyTurn = true;

    public bool Turn
    {
        get => _turn;
        set
        {
            _turn = value;
		    TileList[0].UpdateGridAvailable();
            
            
            if (EnemyBotEnabled && _enemyTurn == _turn)
            {
                
                EnemyRandomTurn();
                
            }
        }
    }

    private void EnemyRandomTurn()
    {
        
        List<Point> availableTiles = new();
        
        
        foreach (var tile in  TileList)
        {
            if (tile.TileColor != Tile.ColorEnum.Available) continue;
            
            availableTiles.Add(tile.Coordinate);
        }

        Random rnd = new();
        
        int randomIndex = rnd.Next(availableTiles.Count);

        foreach (var tile in TileList)
        {
            if (tile.Coordinate != availableTiles[randomIndex]) continue;
            
            
            tile.Tile_Click();
        }

    }
    
    
    private bool _gameOver;
    
    public Tile.ColorEnum Winner;
    
    private int _finalWhiteTilesCount;
    private int _finalBlackTilesCount;

    public void SetGameResults(int whiteTilesCount, int blackTilesCount)
    {
        _finalBlackTilesCount = blackTilesCount;
        _finalWhiteTilesCount = whiteTilesCount;
        _gameOver = true;
        if (blackTilesCount > whiteTilesCount)
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



        void FillCenter4TilesList()
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
        }
        FillCenter4TilesList();
        
        
        TileList = new ObservableCollection<Tile>();

        for (int y = 0; y < GridRows; y++)
        {
            for (int x = 0; x < GridRows; x++)
            {
                TileList.Add(new Tile(x,y, this));
            }
        }
        
        
        TileList[0].UpdateGridAvailable();


    }



}