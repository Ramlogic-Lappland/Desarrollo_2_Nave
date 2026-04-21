using UnityEngine;

public class PlanetBehaviour : MonoBehaviour
{
    [Header("Destruction Effects")]
    [SerializeField] private GameObject destructionEffect; // explosion VFX
    [SerializeField] private bool destroyMoonsOnDeath = true;

    public void DestroyPlanet()
    {
        if (destroyMoonsOnDeath)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    // Optional: For debugging or triggering by other means
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship"))
        {
            DestroyPlanet();
            // DestroyShip();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ship"))
        {
            DestroyPlanet();
        }
    }
}
