using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickupable : MonoBehaviour
{
    [Header("Pickup Settings")] public Transform defaultHoldPoint;

    [Tooltip("Optional positional/rotational offset when snapped to the hold point.")]
    public Vector3 localPositionOffset;

    public Vector3 localEulerOffset;

    [Tooltip("If true, we'll zero velocity and set the rigidbody to kinematic when picked up.")]
    public bool makeKinematicOnPickup = true;

    [Tooltip("If true, we'll restore previous kinematic/gravity settings on drop.")]
    public bool restorePhysicsOnDrop = true;

    Rigidbody _rb;
    bool _isHeld;
    bool _prevKinematic;
    bool _prevUseGravity;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public bool IsHeld => _isHeld;
    public Rigidbody Rigidbody => _rb;

    public void Pickup(Transform holdPoint)
    {
        if (_isHeld) return;
        _isHeld = true;

        // Save physics state
        _prevKinematic = _rb.isKinematic;
        _prevUseGravity = _rb.useGravity;

        if (makeKinematicOnPickup)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        // Parent and snap
        var target = defaultHoldPoint != null ? defaultHoldPoint : holdPoint;
        transform.SetParent(target, false);
        transform.localPosition = localPositionOffset;
        transform.localRotation = Quaternion.Euler(localEulerOffset);
    }

    public void Drop()
    {
        if (!_isHeld) return;
        _isHeld = false;

        transform.SetParent(null, true);

        if (restorePhysicsOnDrop)
        {
            _rb.isKinematic = _prevKinematic;
            _rb.useGravity = _prevUseGravity;
        }
    }
}