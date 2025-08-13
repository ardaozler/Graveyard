using UnityEngine;

[DisallowMultipleComponent]
public class Highlightable : MonoBehaviour
{
    [ColorUsage(showAlpha: false, hdr: true)]
    public Color emissionColor = new Color(1.0f, 0.85f, 0.2f) * 2.0f; // bright warm

    [Tooltip("Leave empty to auto-collect all renderers in children.")]
    public Renderer[] renderers;

    [Tooltip("If assigned, will only highlight when this interactable is currently usable.")]
    public MonoBehaviour interactableComponent;

    MaterialPropertyBlock _mpb;
    bool _isHighlighted;

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(includeInactive: false);
        _mpb = new MaterialPropertyBlock();
    }

    public void SetHighlighted(bool on, PlayerInteractor interactor = null)
    {
        // If linked to an interactable, check if it can be interacted with
        if (on && interactableComponent is IInteractable interactable && interactor != null)
        {
            if (!interactable.CanInteract(interactor))
                on = false;
        }

        _isHighlighted = on;

        foreach (var r in renderers)
        {
            if (!r) continue;
            r.GetPropertyBlock(_mpb);
            if (on)
            {
                _mpb.SetColor("_EmissionColor", emissionColor);
                r.material.EnableKeyword("_EMISSION");
            }
            else
            {
                _mpb.SetColor("_EmissionColor", Color.black);
                r.material.DisableKeyword("_EMISSION");
            }
            r.SetPropertyBlock(_mpb);
        }
    }
}