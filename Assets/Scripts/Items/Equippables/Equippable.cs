using UnityEngine;

public class Equippable : Pickupable
{
    public ToolDefinition tool;

    // === New bits ===
    protected override void AfterPickup(PlayerInteractor interactor)
    {
        if (!tool || !interactor) return;
        var eq = interactor.GetComponent<PlayerEquipment>();
        if (!eq) return;

        // Avoid double-equip if already equipped
        if (!eq.HasTool(tool)) eq.Equip(tool);
    }

    protected override void AfterDrop(PlayerInteractor interactor)
    {
        if (!tool || !interactor) return;
        var eq = interactor.GetComponent<PlayerEquipment>();
        if (!eq) return;

        // Only unequip if this tool is currently equipped
        if (eq.HasTool(tool)) eq.Unequip();
    }
}