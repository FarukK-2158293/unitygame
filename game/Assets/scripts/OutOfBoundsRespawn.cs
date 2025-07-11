using UnityEngine;

public class OutOfBoundsRespawn : MonoBehaviour
{
    public Transform respawnPoint;
    public float fallThreshold = -8.5f; 

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = transform.position;
        if (playerPos.y <= fallThreshold)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        transform.position = respawnPoint.position;
    }
}
