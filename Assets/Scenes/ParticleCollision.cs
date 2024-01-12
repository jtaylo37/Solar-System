using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public AudioClip collisionSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is a particle or nucleus
        if (collision.gameObject.CompareTag("Particle") || collision.gameObject.CompareTag("Sun"))
        {
            if (collisionSound != null)
            {
                Debug.Log("Playing sound due to collision with: " + collision.gameObject.name);
                audioSource.PlayOneShot(collisionSound);
            }
        }
    }
}