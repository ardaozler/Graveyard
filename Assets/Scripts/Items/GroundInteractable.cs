using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[AddComponentMenu("Interaction/GroundInteractable (SO Requirements)")]
public class GroundInteractable : MonoBehaviour, IInteractable
{
    [Header("Requirements (all must pass)")]
    public InteractionRequirement[] requirements;

    [Header("Events")]
    public UnityEvent onDig;

    public string GetPrompt(PlayerInteractor interactor)
    {
        if (CanInteract(interactor)) return "Dig";
        // Show the first failing reason (optional)
        var reason = requirements?.FirstOrDefault(r => r && !r.IsSatisfied(interactor))?.GetBlockedReason(interactor);
        return string.IsNullOrEmpty(reason) ? "Unavailable" : reason;
    }

    public bool CanInteract(PlayerInteractor interactor)
    {
        if (requirements == null || requirements.Length == 0) return true;
        foreach (var r in requirements)
        {
            if (r == null) continue;
            if (!r.IsSatisfied(interactor)) return false;
        }
        return true;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!CanInteract(interactor)) return;
        onDig?.Invoke();
    }
}