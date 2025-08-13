using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Interaction/SimpleUse")]
public class SimpleUse : MonoBehaviour, IInteractable
{
    [Header("Prompt")] public string prompt = "Use";

    [Header("Events")] public UnityEvent onInteract;

    public string GetPrompt(PlayerInteractor interactor) => prompt;

    public bool CanInteract(PlayerInteractor interactor) => true;

    public void Interact(PlayerInteractor interactor)
    {
        onInteract?.Invoke();
    }
}