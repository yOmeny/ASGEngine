using System;
using Scriptables;
using UnityEngine;

public sealed class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] public BulletData Data;

    private float _magnusMultiplier;// constant hop up power (for tuning)

    public float airDensity = 1.2f;       // kg/m^3
    public float maxLift = 0.08f;         // N
    public float minSpeed = 3f;
    public const float radius = 0.002975f;
    public float dragCoefficient = 0.45f;

    float area = Mathf.PI * radius * radius;

    private void SetInitialRigidbodyState()
    {
        _rigidbody.useGravity = true;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.maxAngularVelocity = 10000f;
    }

    public void Initialize(Vector3 initialVelocity, Vector3 initialRotation, float magnusMultiplier)
    {
        SetInitialRigidbodyState();
       
        _rigidbody.linearVelocity = initialVelocity;
        _rigidbody.angularVelocity = initialRotation;
        Debug.Log($"Force:{initialVelocity}");
        Debug.Log($"Rotation:{initialRotation}");
        Debug.Log($"omega={_rigidbody.angularVelocity.magnitude} rad/s, vmax={_rigidbody.maxAngularVelocity}");

        _magnusMultiplier = magnusMultiplier;
    }

    private void FixedUpdate()
    {
        Vector3 v = _rigidbody.linearVelocity;
        float speed = v.magnitude;
        if (speed < minSpeed) return;

     

        Vector3 omega = _rigidbody.angularVelocity;        // rad/s (world)
        float omegaMag = omega.magnitude;
        if (omegaMag < 1e-4f) return;

        // Spin parameter
        float S = (omegaMag * radius) / speed;

        // Lift coefficient (simple saturating curve)
        float Cl = _magnusMultiplier * (S / (1f + S));
        //float Cl = _magnusMultiplier * S;

        // Lift magnitude
        float liftMag = 0.5f * airDensity * speed * speed * area * Cl;
        liftMag = Mathf.Min(liftMag, maxLift);

        // Direction: make lift perpendicular to velocity & spin axis

        Vector3 liftDir = Vector3.Cross(v, omega).normalized;
        _rigidbody.AddForce(liftDir * liftMag, ForceMode.Force);


    }
    
    private void OnCollisionEnter(Collision other)
    {
        UnityEngine.Debug.Log($"Collided with {other.gameObject.name}");
    }
}
