using UnityEngine;

[DisallowMultipleComponent]
public class Highlightable : MonoBehaviour
{
    [ColorUsage(showAlpha: false, hdr: true)]
    public Color emissionColor = new Color(1.0f, 0.85f, 0.2f) * 2.0f;

    private Renderer[] renderers;

    MaterialPropertyBlock _mpb;
    bool _isHighlighted;

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(includeInactive: false);
        _mpb = new MaterialPropertyBlock();
    }

    public void SetHighlighted(bool on)
    {
        if (_isHighlighted == on) return;
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