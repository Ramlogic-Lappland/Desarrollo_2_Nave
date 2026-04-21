using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private float speed = 150f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private GameObject impactEffect; // explosion prefab
    
    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy bullet after lifetime seconds
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Planet"))
        {
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }
            
            Destroy(other.gameObject);  // Destroy the planet
            Destroy(gameObject);// Destroy the bullet
        }
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
