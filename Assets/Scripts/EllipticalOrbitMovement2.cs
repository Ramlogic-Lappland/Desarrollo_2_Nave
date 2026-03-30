using UnityEngine;

public class EllipticalOrbitMovement2 : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private float semiMajorAxis = 10f; // Renamed for clarity
    [SerializeField] private float eccentricity = 0.5f; // Replaces semiMinor (0 = circle, <1 = ellipse)
    [SerializeField] private float orbitalPeriod = 10f; // Seconds to complete one orbit
    [SerializeField] private float angleOffset = 0f; // Starting position

    [Header("Orbit Orientation")] [SerializeField]
    private Vector3 orbitNormal = Vector3.up; // Axis of rotation

    [SerializeField] private float inclination = 0f; // Tilt relative to reference plane

    private float _currentAngle = 0f;
    private float _semiMinorAxis; 

    private void Start()
    {
        _semiMinorAxis = semiMajorAxis * Mathf.Sqrt(1 - Mathf.Pow(eccentricity, 2));
        _currentAngle = angleOffset;
    }

    private void Update()
    {
        if (center == null) return; 
        
        var angularSpeed = 360f / orbitalPeriod; 
        _currentAngle += angularSpeed * Time.deltaTime;
        
        var rad = _currentAngle * Mathf.Deg2Rad;

        var x = semiMajorAxis * Mathf.Cos(rad);
        var y = _semiMinorAxis * Mathf.Sin(rad);

        var localOffset = new Vector3(x, y, 0);
        
        var orbitRotation = Quaternion.FromToRotation(Vector3.up, orbitNormal);
        
        var inclinationRotation = Quaternion.AngleAxis(inclination, Vector3.right);

        var worldOffset = orbitRotation * inclinationRotation * localOffset;
        
        transform.position = center.position + worldOffset;
        
        if (worldOffset.magnitude > 0.01f)
        {
            Vector3 forward = Vector3.Cross(orbitNormal, worldOffset).normalized;
            transform.rotation = Quaternion.LookRotation(forward, orbitNormal);
        }
    }
}