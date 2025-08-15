using UnityEngine;

public abstract class InteractionRequirement : ScriptableObject
{
    // Return true if interactor may use this
    public abstract bool IsSatisfied(PlayerInteractor interactor);

    // Optional short message for prompts (e.g., "Requires Shovel")
    public virtual string GetBlockedReason(PlayerInteractor interactor) => "Unavailable";
}