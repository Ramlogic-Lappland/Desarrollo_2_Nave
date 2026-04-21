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
            var planet = other.GetComponent<PlanetBehaviour>();
            if (planet != null)
            {
                planet.DestroyPlanet();
            }
            else
            {
                Destroy(other.gameObject);
            }

            Destroy(gameObject);
        }
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
