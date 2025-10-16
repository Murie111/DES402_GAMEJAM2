using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform player;
    public float distance = 10;

    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, distance, 0);
    }
}
