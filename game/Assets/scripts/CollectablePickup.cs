using UnityEngine;

public class Pickup : MonoBehaviour
{
    public AudioClip pickupSound;
    public PowerItem powerEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                powerEffect.Apply(other.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
