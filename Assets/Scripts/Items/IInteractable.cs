public interface IInteractable
{
    // Return a short UI string like "Open", "Press", "Use" (can be null/empty)
    string GetPrompt(PlayerInteractor interactor);

    bool CanInteract(PlayerInteractor interactor);

    void Interact(PlayerInteractor interactor);
}