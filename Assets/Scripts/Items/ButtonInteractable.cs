using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Interaction/ButtonInteractable")]
public class ButtonInteractable : MonoBehaviour, IInteractable
{
    public string prompt = "Press";
    public float cooldown = 0.25f;
    public UnityEvent onPressed;

    float _readyAt;

    public string GetPrompt(PlayerInteractor interactor) => prompt;

    public bool CanInteract(PlayerInteractor interactor) => Time.time >= _readyAt;

    public void Interact(PlayerInteractor interactor)
    {
        if (!CanInteract(interactor)) return;
        _readyAt = Time.time + cooldown;
        onPressed?.Invoke();
    }
}
