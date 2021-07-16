using UnityEngine;

public class MazeCell
{
    public int row, col;
    public bool visited;

    public GameObject northWall, eastWall, southWall, westWall, floor; // a maze cell consists of 4 walls, one for each direction and a floor

    public MazeCell(int row, int col)
    {
        this.row = row;
        this.col = col;
        visited = false;
    }
}
