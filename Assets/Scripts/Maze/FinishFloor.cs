using UnityEngine;

public class FinishFloor : MonoBehaviour
{
    private GameStateHandler handler;

    void Awake()
    {
        handler = GameObject.FindWithTag("GameController").GetComponent<GameStateHandler>();
        handler.SetFinishPosition(transform.position);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //if player enters the finish zone call the Win method in the Game state handler
            handler.Win();
            Destroy(GetComponent<BoxCollider>());
        }
    }
}
