using UnityEngine;

public class SunBehaviour : MonoBehaviour
{
    [Header("Destruction Effects")]
    [SerializeField] private GameObject destructionEffect; // explosion VFX
    [SerializeField] private SpaceShipScript spaceShip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship"))
        {
            spaceShip.Death();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ship"))
        {
            spaceShip.Death();
        }
    }
}
