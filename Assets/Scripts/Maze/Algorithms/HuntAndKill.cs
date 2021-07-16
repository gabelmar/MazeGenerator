using UnityEngine;

/*
    Implementation of the hunt and kill maze generation algorithm. It is pretty similiar to the recursive backtracker but instead of
    using a stack to backtrack when it gets stuck in a dead end, it scans the whole maze from top to bottom, left to right to find a 
    cell that is unvisited but has a visited neighbor cell (this is the hunt phase). If it finds one this becomes the current cell and the kill phase begins again.
    In the kill phase it randomly chooses a unvisited neighbor cell, destroys the wall between the current and the chosen cell and makes it the current cell.
    It continues to do so. Finally when it cant find anymore unvisited neighbor cells, it reaches a "dead end". Then it goes into hunt phase as described already.
    When there are no available cells found in the hunt phase the maze is complete.
    This algorithm can be faster compared to the recursive backtracker especially for larger mazes where the stack gets quite large in size and backtracking can 
    become time consuming.
 */
public class HuntAndKill : IMazeAlgorithm
{
    private MazeCell[,] maze;
    private int rows;
    private int cols;
    private int currentRow;
    private int currentCol;
    private bool mazeComplete;
    
    public HuntAndKill(MazeCell[,] maze, int rows, int cols)
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
        while(!mazeComplete)
        {
            Kill();
            Hunt();
        }
    }

    /* In the kill phase the algorithm randomly chooses a neigbor cell of the current cell, destroys the walls between them and
    makes the neighbor cell the current one. It continues to do so while there are neighbor cells available */
    private void Kill()
    {
        while (NeighborCellsAvailable(currentRow, currentCol)){
            int direction = Random.Range(0,4);

            // North, destroy north wall and make the cell in the row above the current one so decrease current row by 1
            if (direction == 0 && CellAvailable(currentRow - 1, currentCol)){
                GameObject.Destroy(maze[currentRow, currentCol].northWall);
                currentRow--;
            }
            // East, destroy east wall and make the cell in the next col the current one so increase current col by 1
            else if (direction == 1 && CellAvailable(currentRow, currentCol + 1)){
                GameObject.Destroy(maze[currentRow, currentCol].eastWall);
                currentCol++;
            }
            // South, destroy south wall and make the cell in the row below the current one so increase row by 1
            else if (direction == 2 && CellAvailable(currentRow + 1, currentCol)){
                GameObject.Destroy(maze[currentRow, currentCol].southWall);
                currentRow++;
            }
            // West, destroy west wall and make the cell in the col before the current one so decrease col by 1
            else if (direction == 3 && CellAvailable(currentRow, currentCol - 1)){
                GameObject.Destroy(maze[currentRow, currentCol].westWall);
                currentCol--;
            }
            // mark the current cell as visited
            maze[currentRow, currentCol].visited = true;
        }
    }

    private bool NeighborCellsAvailable(int row, int col)
    {
        // north available
        if(row > 0 && !maze[row-1, col].visited)
            return true;
        // east available
        if(col < cols - 1 && !maze[row, col+1].visited)
            return true;
        // south available
        if(row < rows - 1 && !maze[row+1, col].visited)
            return true;
        // west available
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

    private void Hunt()
    {
        mazeComplete = true;
        for (int row = 0; row < rows; row++){
            for (int col = 0; col < cols; col++){
                // if a cell is found that is unvisited but has a visited neighbor make it the current cell
                if(!maze[row, col].visited && HasVisitedNeighborCell(row,col)){
                    mazeComplete = false;
                    currentRow = row;
                    currentCol = col;
                    // destroy the wall between them or a random one if there are multiple possibilities
                    DestroyAdjacentWall(currentRow, currentCol);
                    maze[currentRow, currentCol].visited = true;
                    return;
                }
            }
        }
    }

    /* This method checks if there are any visited neighbor cells to the cell that is passed in. It returns true as soon as it finds one.
    So there can be only one or multiple ones */
    private bool HasVisitedNeighborCell(int row, int col)
    {
        //north has been visited
        if(row > 0 && maze[row-1, col].visited)
            return true;
        //east has been visited
        if(col < cols - 1 && maze[row, col+1].visited)
            return true;
        //south has been visited
        if(row < rows - 1 && maze[row+1, col].visited)
            return true;
        //west has been visited
        if(col > 0 && maze[row, col - 1].visited)
            return true;
        return false;
    }

    /* This method is called during the hunt phase. It destroys a wall between the current cell that is unvisited and a neighbor cell
    that is visited. */
    private void DestroyAdjacentWall(int row, int col)
    {
        bool wallDestroyed = false;
        while(!wallDestroyed){
            int direction = Random.Range(0,4);
            //North
            if (direction == 0 && row > 0 && maze[row - 1, col].visited){
                GameObject.Destroy(maze[row, col].northWall);
                wallDestroyed = true;
            }
            //East
            else if (direction == 1 && col < cols - 2 && maze[row, col + 1].visited){
                GameObject.Destroy(maze[row, col].eastWall);
                wallDestroyed = true;
            }
            //South
            else if (direction == 2 && row < rows - 2 && maze[row + 1, col].visited){
                GameObject.Destroy(maze[row, col].southWall);
                wallDestroyed = true;
            }
            //West
            else if (direction == 3 && col > 0 && maze[row, col - 1].visited){
                GameObject.Destroy(maze[row, col].westWall);
                wallDestroyed = true;
            }   
        }
    }
}
