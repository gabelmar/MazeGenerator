using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionArrow : MonoBehaviour
{
    [SerializeField] private Transform playerPos;
    private SpriteRenderer renderer;

    void Start()
    {
        playerPos = GameObject.FindWithTag("Player").transform;
        renderer = GetComponent<SpriteRenderer>();
    }

    public void UpdatePosition()
    {
        // set the arrow position to the players position but offset y by 5 to make the arrow float above the player
        Vector3 pos = playerPos.position;
        pos.y += 5;
        transform.position = pos;

        // rotate the arrow by the players rotation, set x to 90 degrees so that the arrow sprite is visible from looking from top
        // always offset y by -90 degrees to make player and arrow point in the same direction (arrow sprite was pointing right initially, player up)
        Vector3 rot = playerPos.rotation.eulerAngles;
        rot.x = 90f;
        rot.y -= 90f;
        transform.rotation = Quaternion.Euler(rot);
    }
    public void Show()
    {
        renderer.enabled = true;
    }

    public void Hide()
    {
        renderer.enabled = false;
    }
}
