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

    private Collider[] _colliders;

    void Awake() => _rb = GetComponent<Rigidbody>();

    public bool IsHeld => _isHeld;
    public Rigidbody Rigidbody => _rb;

    public void Pickup(Transform holdPoint, PlayerInteractor interactor)
    {
        if (_isHeld) return;
        _isHeld = true;

        _colliders = transform.GetComponentsInChildren<Collider>();

        _prevKinematic = _rb.isKinematic;
        _prevUseGravity = _rb.useGravity;

        if (makeKinematicOnPickup)
        {
            _rb.linearVelocity = Vector3.zero; // use velocity/angularVelocity
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }

        foreach (var collider in _colliders)
        {
            collider.enabled = false;
        }

        var target = defaultHoldPoint != null ? defaultHoldPoint : holdPoint;
        transform.SetParent(target, false);
        transform.localPosition = localPositionOffset;
        transform.localRotation = Quaternion.Euler(localEulerOffset);

        AfterPickup(interactor);
    }

    public void Drop(PlayerInteractor interactor)
    {
        if (!_isHeld) return;
        _isHeld = false;

        transform.SetParent(null, true);

        if (restorePhysicsOnDrop)
        {
            _rb.isKinematic = _prevKinematic;
            _rb.useGravity = _prevUseGravity;
        }

        foreach (var collider in _colliders)
        {
            collider.enabled = true;
        }

        AfterDrop(interactor);
    }

    protected virtual void AfterPickup(PlayerInteractor interactor)
    {
    }

    protected virtual void AfterDrop(PlayerInteractor interactor)
    {
    }
}