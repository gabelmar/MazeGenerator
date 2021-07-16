using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    // reference all the UI elements and cameras
    [SerializeField] private Camera mainCamera, playerCamera;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Slider rowSlider;
    [SerializeField] private Slider colSlider;
    [SerializeField] private Text rowValue;
    [SerializeField] private Text colValue;
    [SerializeField] private Text algorithm;
    [SerializeField] private Text time;
    private MazeGenerator generator; // the ui handler has to know about the maze generator and pass the input values to it
    private GameStateHandler stateHandler;
    private CameraScript cameraScript; // the ui handler has to have access to the camera to make correct scaling / sizing of the maze possible
    private Stopwatch watch; // the c# stopwatch class to measure the time that was needed to generate the maze
    private string switchModesInputName;
    // just like in the player controller i referenece the name of the input key to avoid generating new string objects in evrey update call
    // furthermore it makes making changes to the controls much easier ( e.g. player changing controls at runtime)
    void Start()
    {
        // assign the correct components to the field references
        generator = GetComponent<MazeGenerator>();
        stateHandler = GetComponent<GameStateHandler>();
        cameraScript = mainCamera.GetComponent<CameraScript>();
        rowSlider.value = generator.GetRows();
        rowValue.text = generator.GetRows().ToString();
        colSlider.value = generator.GetCols();
        colValue.text = generator.GetCols().ToString();
        watch = new Stopwatch();
        mainCamera.enabled = true;
        playerCamera.enabled = false;
        switchModesInputName = "space";
    }

    void Update()
    {
        // if the key to switch between modes is pressed, do it (space by default)
        if (Input.GetKeyDown(switchModesInputName))
        {
            bool mode = stateHandler.IsPlayerMode();
            stateHandler.SetPlayerMode(!mode);
            // switch bewteen cameras accordingly, could be moved to the game state handler maybe
            mainCamera.enabled = !mainCamera.enabled;
            playerCamera.enabled = !playerCamera.enabled;
        }
    }

    // the following methods are bound to the buttons, sliders and dropdown menu on the UI and are called when their values change
    public void OnRowsChanged()
    {
        int value = (int)rowSlider.value;
        rowValue.text = value.ToString();
        generator.SetChangedRows(value);
    }

    public void OnColsChanged()
    {
        int value = (int)colSlider.value;
        colValue.text = value.ToString();
        generator.SetChangedCols(value);
    }

    public void OnGenerate()
    {
        // update camera with new row and col values and generate the maze
        cameraScript.AdjustCamera((int)rowSlider.value, (int)colSlider.value, generator.GetCellSize());
        // Reset and start the stopwatch, generate the maze and then stop the time
        watch.Reset();
        watch.Start();
        generator.GenerateMaze();
        watch.Stop();
        // update the time display with the new time
        UpdateTime(watch.Elapsed.Milliseconds);

        // reset teh players position to the start
        stateHandler.Respawn();
    }

    public void OnReset()
    {
        // when the reset button is clicked the player is set back to the start position and the maze gets reset to a full grid
        stateHandler.Respawn();
        generator.Reset();
    }

    public void OnAlgorithmChanged()
    {
        generator.SetAlgorithm(algorithm.text);
    }

    private void UpdateTime(int milliseconds)
    {
        time.text = "Time(ms): " + milliseconds;
    }

    // methods to hide and show the UI, so the canvas it is laying on
    public void Hide()
    {
        canvas.SetActive(false);
    }

    public void Show()
    {
        canvas.SetActive(true);
    }
}
