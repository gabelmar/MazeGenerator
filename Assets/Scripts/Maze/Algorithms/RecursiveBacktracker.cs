using System.Collections.Generic;
using UnityEngine;

/*
    Implementation of the recursive backtracker algorithm as it can be found on the wikipedia page (https://en.wikipedia.org/wiki/Maze_generation_algorithm)
    using a stack to keep track of where it went to be able to backtrack and continue from an earlier position
 */
public class RecursiveBacktracker : IMazeAlgorithm
{
    private MazeCell[,] maze;
    private int rows;
    private int cols;
    private int currentRow;
    private int currentCol;
    private bool mazeComplete;
    
    public RecursiveBacktracker(MazeCell[,] maze, int rows, int cols)
    {
        this.maze = maze;
        this.rows = rows;
        this.cols = cols;
        currentRow = 0;
        currentCol = 0;
        maze[currentRow,currentCol].visited = true;
        mazeComplete = false;
    }
    
    public void CreateMaze()
    {
        Stack<MazeCell> stack = new Stack<MazeCell>();

        while(!mazeComplete)
        {
            // check if there are neighbor cells available
            if(NeighborCellsAvailable(currentRow, currentCol)){
                // if so generate a random number (direction) to determine the random neighbor cell
                int direction = Random.Range(0,4);
                // push current to stack
                stack.Push(maze[currentRow, currentCol]);

                // then for each possible direction check if the randomly chosen neighbor cell is available if so destroy the walls to it and make it the current
                // this could probably be done a bit more efficiently by just generating a random cell out of a pool of the available cells and not
                // out of all 4 every time, but as its just 4 neighbors anyway it's easier to do like this and shouldn't be too much of a time loss
                // the same applies for the hunt and kill algorithm by the way where I am doing it basically the same way

                // North
                if (direction == 0 && CellAvailable(currentRow - 1, currentCol))
                {
                    GameObject.Destroy(maze[currentRow, currentCol].northWall);
                    currentRow--;
                }
                // East
                else if (direction == 1 && CellAvailable(currentRow, currentCol + 1))
                {
                    GameObject.Destroy(maze[currentRow, currentCol].eastWall);
                    currentCol++;
                }
                // South
                else if (direction == 2 && CellAvailable(currentRow + 1, currentCol))
                {
                    GameObject.Destroy(maze[currentRow, currentCol].southWall);
                    currentRow++;
                }
                // West
                else if (direction == 3 && CellAvailable(currentRow, currentCol - 1))
                {
                    GameObject.Destroy(maze[currentRow, currentCol].westWall);
                    currentCol--;
                }

            maze[currentRow, currentCol].visited = true;
            }
            // if the stack is not empty pop a cell and make it the current cell
            else if(!(stack.Count == 0))
            {
                MazeCell current = stack.Pop();
                currentRow = current.row;
                currentCol = current.col;
            }
            // if there are no neighbor cells availabe anymore and there's nothing left in the stack to go back to, the maze is complete so the loop ends
            else 
            {
                mazeComplete = true;
            }
        }    
    }

    // the method checks if there are any neighbor cells available and just that. It doesnt provide information about which cells are avialable
    // just that there is at least one
    private bool NeighborCellsAvailable(int row, int col)
    {
        //north available
        if(row > 0 && !maze[row-1, col].visited)
            return true;
        //east available
        if(col < cols - 1 && !maze[row, col+1].visited)
            return true;
        //south available
        if(row < rows - 1 && !maze[row+1, col].visited)
            return true;
        //west available
        if(col > 0 && !maze[row, col - 1].visited)
            return true;
        return false;
    }


    // helper method to check if a cell is in bounds and not visited
    private bool CellAvailable(int row, int col)
    {
        if(row >= 0 && row < rows && col >= 0 && col < cols && !maze[row, col].visited)
            return true;
        else 
            return false;
    }
    
    
    
}
