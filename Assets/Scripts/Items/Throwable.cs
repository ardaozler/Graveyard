using UnityEngine;

[RequireComponent(typeof(Pickupable))]
[RequireComponent(typeof(Rigidbody))]
public class Throwable : MonoBehaviour
{
    [Header("Throw Settings")] public float throwForce;
    public float throwUpwardBias;

    Pickupable _pickupable;
    Rigidbody _rb;

    void Awake()
    {
        _pickupable = GetComponent<Pickupable>();
        _rb = GetComponent<Rigidbody>();
    }

    public void Throw(Vector3 direction)
    {
        // Drop first so physics applies
        _pickupable.Drop();

        // Add force after dropping
        Vector3 dir = direction.normalized;
        dir += Vector3.up * throwUpwardBias;
        _rb.AddForce(dir.normalized * throwForce, ForceMode.VelocityChange);
    }
}