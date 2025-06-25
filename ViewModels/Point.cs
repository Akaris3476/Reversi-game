namespace Reversi_game.ViewModels;

public struct Point
{
	public int X { get; }
	public int Y { get; }

	public Point(int x, int y)
	{
		X = x;
		Y = y;
	}
	
	public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
	public static bool operator !=(Point a, Point b) => !(a == b);

	public override string ToString() => $"X: {X}, Y: {Y}";

}