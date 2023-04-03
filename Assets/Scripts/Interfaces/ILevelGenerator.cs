using Data;
using UnityEngine;

public interface ILevelGenerator
{
    public (int, int) DrawLine(int x, int y, int length, int thickness, LineDirection lineDirection);
    public (int, int, LineDirection) GenerateLines(int x, int y, int thickness, LineDirection startDirection);
    public Ground[,] GenerateGround(int width, int height, int thickness, DiamondsOrder diamondsOrder);

    public Vector2 GetSpawnPoint();

    public Ground[,] MoveLevelDown();
    int MoveDownHeight { get; }
}