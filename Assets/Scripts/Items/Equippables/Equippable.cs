using UnityEngine;

[AddComponentMenu("Interaction/Equippable (SO Tool)")]
public class Equippable : Pickupable, IInteractable
{
    public ToolDefinition tool; 

    public string GetPrompt(PlayerInteractor interactor)
    {
        var eq = interactor ? interactor.GetComponent<PlayerEquipment>() : null;
        if (!tool) return "Equip";
        if (eq && eq.HasTool(tool)) return $"Unequip {tool.displayName}";
        return $"Equip {tool.displayName}";
    }

    public bool CanInteract(PlayerInteractor interactor) => tool != null;

    public void Interact(PlayerInteractor interactor)
    {
        if (!tool) return;
        var eq = interactor ? interactor.GetComponent<PlayerEquipment>() : null;
        if (!eq) return;

        if (eq.HasTool(tool)) eq.Unequip();
        else eq.Equip(tool);
    }
}