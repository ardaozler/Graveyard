using UnityEngine;

public partial class PlayerInteractor : MonoBehaviour
{
    public PlayerInventory Inventory;

    void LateUpdate()
    {
        HandleEquipHotkeys();
    }

    void HandleEquipHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_held == null)
            {
                if (Inventory && Inventory.ActiveKind == ToolKind.Lantern) Inventory.Unequip();
                else if (Inventory && Inventory.CanEquip(ToolKind.Lantern, false)) Inventory.Equip(ToolKind.Lantern);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_held == null)
            {
                if (Inventory && Inventory.ActiveKind == ToolKind.Shovel) Inventory.Unequip();
                else if (Inventory && Inventory.CanEquip(ToolKind.Shovel, false)) Inventory.Equip(ToolKind.Shovel);
            }
        }
        if (Input.GetKeyDown(throwKey) && Inventory && Inventory.ActiveKind == ToolKind.Lantern)
        {
            var dir = _cam ? _cam.transform.forward : transform.forward;
            Inventory.ThrowLantern(dir);
        }
    }

    void InventoryAware_PrePickup()
    {
        if (Inventory && Inventory.HasAnyToolEquipped)
        {
            Inventory.AutoStowActiveTool();
        }
    }
}