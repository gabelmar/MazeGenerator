using UnityEngine;

public class GameStateHandler : MonoBehaviour
{
    [SerializeField] private GameObject player; // reference to the player, mainly to have acccess to its controller
    [SerializeField] private GameObject arrow; // an arrow (sprite) showing the players position in the maze
    [SerializeField] private GameObject confetti; // the particle effect that appears when the player makes it to the end of the maze

    private UIHandler ui;
    private PlayerController3D playerController; // referencing the PlayerController once as a field to avoid having to get the component in every update call
    private PlayerPositionArrow arrowScript;
    private bool playerMode;
    private Vector3 finishPosition;

    void Start()
    {
        playerMode = false;
        ui = GetComponent<UIHandler>();
        arrowScript = arrow.GetComponent<PlayerPositionArrow>();
        playerController = player.GetComponent<PlayerController3D>();
    }

    void Update()
    {
        // disable player controls when not in player mode / enable when in player mode
        playerController.enabled = playerMode;

        //when in player mode hide the UI and the player position
        if (playerMode)
        {
            ui.Hide();
            arrowScript.Hide();
            // update the position of the arrow showing the players position
            arrowScript.UpdatePosition();
        }

        // when not in player mode display both the UI and the players position
        else
        {
            ui.Show();
            arrowScript.Show();
        }

    }

    public bool IsPlayerMode() => playerMode;

    public void SetPlayerMode(bool mode)
    {
        playerMode = mode;
    }
    public void Respawn()
    {
        // kind of a dirty trick to solve the problem of the character controller ignoring all the changes to the players transform from the outside
        // by default the character controller will hold the player in the same position and it can only be moved by calling Move or SimpleMove
        // to work around this the character controller is disabled, then the player is moved and afterwards the character controller gets reenabled 
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = new Vector3(1, 1, -1);
        player.GetComponent<CharacterController>().enabled = true;
        // Update the arrow position manually because we aren't in player mode, so that you can still see the players position in the maze
        arrowScript.UpdatePosition();
    }

    public void Win()
    {
        // Spawn the confetti particle effect in the center of the finish floor slightly above the player
        Vector3 position = finishPosition;
        position.y += 5;
        Instantiate(confetti, position, Quaternion.identity);
    }

    public void SetFinishPosition(Vector3 position)
    {
        // called by the finish floor
        // set the finish position so that the game handler knows where to spawn the confetti effect
        finishPosition = position;
    }
}
