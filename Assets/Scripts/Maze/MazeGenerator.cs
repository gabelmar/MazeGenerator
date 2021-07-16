using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject wall; // prefab for the walls
    [SerializeField] private GameObject floor; // prefab for floors
    [SerializeField] private Material start; // material for the start floor (green)
    [SerializeField] private Material finish; //material for finish floor (red)
    private int rows = 10; // amount of rows in the maze, default 10
    private int cols = 10; // amount of columns in the maze, default 10
    private int changedRows; // field to temprorarily store the changed row value from the UI slider
    private int changedCols; // field to temprorarily store the changed column value from the UI slider
    private string algorithm; // the name of the algorithm to generate the maze with

    private int cellSize = 10; // the side length of one cell in the maze, set to 10 because walls have a length of 10
    private MazeCell[,] maze; // the maze is stored as a two dimensional array of maze cells

    void Start()
    {
        // initialize the changed rows and columns with default values (10)
        changedRows = rows;
        changedCols = cols;
        // default algorithm to hunt and kill
        algorithm = "Hunt and Kill";
        GenerateGrid();
    }

    /* This method initializes the maze cell array and generates a grid of walls which is later used to create the maze.
    To avoid double generating walls (north of one cell = south of the cell above, east of one cell = west of the cell to the right) 
    it only instantiates north walls for the first row and west walls for the first column. Afterwards it sets the correct references to the
    walls as described above so that each cell can access all of its walls.*/
    public void GenerateGrid()
    {
        maze = new MazeCell[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                maze[row, col] = new MazeCell(row, col);

                // floors always
                maze[row, col].floor = Instantiate(floor, new Vector3(cellSize * col, 0, cellSize * -row), Quaternion.identity);
                maze[row, col].floor.transform.Rotate(new Vector3(90, 0, 0));
                // change the game objects name to the element it is representing and its position in the grid for debugging purposes
                maze[row, col].floor.name = "Floor [" + row + ", " + col + "]";

                // north walls, only in the first row
                if (row == 0)
                {
                    maze[row, col].northWall = Instantiate(wall, new Vector3(cellSize * col, cellSize / 2, cellSize * -row + cellSize / 2), Quaternion.identity);
                    maze[row, col].northWall.name = "North [" + row + ", " + col + "]";
                }

                // east walls always
                maze[row, col].eastWall = Instantiate(wall, new Vector3(cellSize * col + cellSize / 2, cellSize / 2, cellSize * -row), Quaternion.identity);
                maze[row, col].eastWall.transform.Rotate(new Vector3(0, 90, 0));
                maze[row, col].eastWall.name = "East [" + row + ", " + col + "]";

                // south walls always
                maze[row, col].southWall = Instantiate(wall, new Vector3(cellSize * col, cellSize / 2, cellSize * -row - cellSize / 2), Quaternion.identity);
                maze[row, col].southWall.name = "South [" + row + ", " + col + "]";

                // west walls, only in the first column
                if (col == 0)
                {
                    maze[row, col].westWall = Instantiate(wall, new Vector3(cellSize * col - cellSize / 2, cellSize / 2, cellSize * -row), Quaternion.identity);
                    maze[row, col].westWall.transform.Rotate(new Vector3(0, 90, 0));
                    maze[row, col].westWall.name = "West [" + row + ", " + col + "]";
                }
            }

        }

        // assign the correct materials for start and goal to the renderer
        maze[0, 0].floor.GetComponent<Renderer>().material = start;
        maze[rows - 1, cols - 1].floor.GetComponent<Renderer>().material = finish;

        MarkFinishCell(maze[rows - 1, cols - 1]);


        // set references to walls
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // north wall is actually the south wall of the cell above (row -1)
                if (row > 0)
                {
                    maze[row, col].northWall = maze[row - 1, col].southWall;
                }
                // west wall is actually the east wall of the cell to the left (col -1)
                if (col > 0)
                {
                    maze[row, col].westWall = maze[row, col - 1].eastWall;
                }
            }
        }
    }

    public void DestroyMaze()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Destroy(maze[row, col].floor);
                Destroy(maze[row, col].northWall);
                Destroy(maze[row, col].eastWall);
                Destroy(maze[row, col].southWall);
                Destroy(maze[row, col].westWall);

            }
        }
    }

    public void Reset()
    {
        DestroyMaze();
        rows = changedRows;
        cols = changedCols;
        GenerateGrid();
    }

    public void GenerateMaze()
    {
        // destroy the maze before generating the new one
        DestroyMaze();
        // update with slider values
        rows = changedRows;
        cols = changedCols;
        //generate the grid
        GenerateGrid();
        // create the maze out of the grid with the chosen algorithm
        IMazeAlgorithm mazeAlgorithm = null;
        switch (algorithm)
        {
            case "Hunt and Kill":
                mazeAlgorithm = new HuntAndKill(maze, rows, cols);
                break;
            case "Recursive BT":
                mazeAlgorithm = new RecursiveBacktracker(maze, rows, cols);
                break;
            default:
                Debug.LogError("Maze Algorithm called " + algorithm + " is unknown");
                break;
        }
        mazeAlgorithm.CreateMaze();
    }

    private void MarkFinishCell(MazeCell cell)
    {
        // Instantiate / Duplicate the floor and make the new one a trigger
        // This is to not make the player fall through the floor
        // instantiate in world space = true to make the new object the exact same as the parent (the floor) since it is sized and rotated in world space
        GameObject finishTrigger = Instantiate(cell.floor, cell.floor.transform, true);
        finishTrigger.AddComponent<Rigidbody>();
        finishTrigger.GetComponent<Rigidbody>().isKinematic = true;
        finishTrigger.GetComponent<Rigidbody>().useGravity = false;
        finishTrigger.GetComponent<BoxCollider>().isTrigger = true;
        // move the box collider a little bit above the ground so that it is above the actual solid floor and bot covered to make it able to recockgnize player collison
        Vector3 center = finishTrigger.GetComponent<BoxCollider>().center;
        center.z -= 0.5f;
        finishTrigger.GetComponent<BoxCollider>().center = center; ;
        finishTrigger.AddComponent<FinishFloor>();
    }

    public int GetRows() => rows;

    public int GetCols() => cols;

    public int GetCellSize() => cellSize;

    public void SetRows(int rows)
    {
        this.rows = rows;
    }
    public void SetCols(int cols)
    {
        this.cols = cols;
    }
    public void SetChangedRows(int rows)
    {
        changedRows = rows;
    }
    public void SetChangedCols(int cols)
    {
        changedCols = cols;
    }
    public void SetAlgorithm(string alg)
    {
        this.algorithm = alg;
    }
}
