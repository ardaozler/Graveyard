using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.Serialization;

public class InteractableWithRequierement : MonoBehaviour, IInteractable
{
    [Header("Requirements (all must pass)")]
    public InteractionRequirement[] requirements;

    [FormerlySerializedAs("onDig")] [Header("Events")]
    public UnityEvent onSuccess;

    public string prompt = "Interact";

    public string GetPrompt(PlayerInteractor interactor)
    {
        if (CanInteract(interactor)) return prompt;
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
        onSuccess?.Invoke();
    }
}