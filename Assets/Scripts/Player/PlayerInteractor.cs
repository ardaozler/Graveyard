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

            if (_held == null)
            {
                TryPickup();
            }
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
                throwable.Throw(dir);
                _held = null;
            }
        }
    }

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
                    pickup.Pickup(holdPoint);
                    _held = pickup;
                }
            }
        }
    }

    void DropHeld()
    {
        if (_held == null) return;
        _held.Drop();
        _held = null;
    }

    void HandleLook()
    {
        _lookInteractable = null;
        Highlightable targetHighlight = null;

        if (_cam && Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit,
                interactDistance, interactMask))
        {
            // Find interactable
            _lookInteractable = hit.collider.GetComponentInParent<IInteractable>();

            // Highlight if not held
            if (!_held)
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