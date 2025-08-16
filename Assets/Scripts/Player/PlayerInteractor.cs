using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast Settings")] public Transform holdPoint;
    public float interactDistance = 3f;
    public LayerMask interactMask = ~0;

    [Header("Input Settings")] public KeyCode interactKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;

    public KeyCode throwKey = KeyCode.Mouse1;

    private Camera _cam;
    private Pickupable _held;
    private Highlightable _lastHighlight;
    private IInteractable _lookInteractable;

    void Awake()
    {
        _cam = Camera.main;
        if (!_cam)
        {
            Debug.LogWarning("PlayerInteractor: No Camera.main found. Assign a camera with the MainCamera tag.");
        }
    }

    void Update()
    {
        HandleLook();
        HandleInteractInput();
        HandleThrowInput();
    }

    void HandleInteractInput()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // If looking at an interactable, use it
            if (_lookInteractable != null && _lookInteractable.CanInteract(this))
            {
                _lookInteractable.Interact(this);
            }

            // Try pickup
            TryPickup();
        }
        else if (Input.GetKeyDown(dropKey))
        {
            DropHeld();
        }
    }

    void HandleThrowInput()
    {
        if (Input.GetKeyDown(throwKey) && _held != null)
        {
            var throwable = _held.GetComponent<Throwable>();
            if (throwable != null)
            {
                Vector3 dir = _cam ? _cam.transform.forward : transform.forward;
                throwable.GetComponent<Pickupable>().Drop(this);
                throwable.Throw(dir);
                _held = null;
            }
        }
    }

    /// <summary>
    /// Tries to pick up the object in front of the player. If has a held object, it will drop it first.
    /// </summary>
    void TryPickup()
    {
        if (!_cam) return;
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit, interactDistance,
                interactMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent(out Pickupable pickup))
            {
                if (pickup != null)
                {
                    DropHeld();
                    pickup.Pickup(holdPoint, this);
                    _held = pickup;
                }
            }
        }
    }

    void DropHeld()
    {
        if (_held == null) return;
        _held.Drop(this);
        _held = null;
    }

    void HandleLook()
    {
        _lookInteractable = null;
        Highlightable targetHighlight = null;

        if (_cam && Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit,
                interactDistance, interactMask))
        {
            _lookInteractable = hit.collider.GetComponentInParent<IInteractable>();

            if (!_held || hit.collider.gameObject != _held.gameObject) 
            {
                targetHighlight = hit.collider.GetComponentInParent<Highlightable>();
            }
        }


        if (_lastHighlight && _lastHighlight != targetHighlight)
            _lastHighlight.SetHighlighted(false, this);

        if (targetHighlight)
        {
            bool can = _lookInteractable == null || _lookInteractable.CanInteract(this);
            targetHighlight.SetHighlighted(can, this);
        }

        _lastHighlight = targetHighlight;
    }
}