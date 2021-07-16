using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MazeGenerator generator = GameObject.Find("GameManager").GetComponent<MazeGenerator>();
        int rows = generator.GetRows();
        int cols = generator.GetCols();
        int cellSize = generator.GetCellSize();
        AdjustCamera(rows, cols, cellSize);
    }

    public void AdjustCamera(int rows, int cols, int cellSize)
    {
        //calculating aspect ratios of screen and maze
        float screenRatio = (float)Screen.width / Screen.height;
        float mazeRatio = (float)(cols * cellSize) / (rows * cellSize);
        float margin = 0.05f; // the desired space betweeen screen borders and the maze in percent

        //Calculating the orthographic size of the camera depending on the aspect ratios of the screen and the maze
        if (screenRatio >= mazeRatio)
        {

            // screenRatio is bigger -> wider than mace -> so the screen borders should be the top and bottom of the maze
            Camera.main.orthographicSize = (rows * cellSize) * 0.5f + margin * (rows * cellSize);
        }
        else
        {

            //maze Ratio is bigger -> the maze is "wider" than the screen ratio so the camera has to adjust to the width of the maze 
            // by calculating the difference between the ratios and multiplying with it
            float difference = mazeRatio / screenRatio;
            Camera.main.orthographicSize = (rows * cellSize) * 0.5f * difference + margin * (cols * cellSize) * 0.5f;
        }
        //puts the camera to the center of the maze at a fixed height, have to subtract half of the cell size to center it in the middle cell itself
        transform.position = new Vector3(cols * cellSize / 2f - cellSize / 2, 53, (rows * cellSize / 2f - cellSize / 2) * -1);
    }
}
