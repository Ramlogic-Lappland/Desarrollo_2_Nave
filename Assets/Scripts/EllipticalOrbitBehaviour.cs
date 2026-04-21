using UnityEngine;

public class EllipticalOrbitBehaviour : MonoBehaviour
{
    [SerializeField] private Transform center;          
    [SerializeField] private float semiMajor = 10f;     
    [SerializeField] private float semiMinor = 5f;      
    [SerializeField] private float angularSpeed = 1f;   
    [SerializeField] private float angle = 0f;

    private void Update()
    {
        if (center == null) return; 
        
        angle += angularSpeed * Time.deltaTime;
        var rad = angle * Mathf.Deg2Rad;
        
        var xAxis = Vector3.right;  
        var yAxis = Vector3.up;        

        var offset = semiMajor * Mathf.Cos(rad) * xAxis + semiMinor * Mathf.Sin(rad) * yAxis;
        transform.position = center.position + offset;
    }
    
}
