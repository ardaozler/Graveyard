using UnityEngine;

[AddComponentMenu("Player/PlayerInventory (Lantern+Shovel)")]
public class PlayerInventory : MonoBehaviour
{
    public int shovelStockAtShack = 3;
    public Transform handSlot;
    public GameObject lanternHandPrefab;
    public GameObject shovelHandPrefab;
    public GameObject lanternBrokenModelPrefab;
    public GameObject shovelBrokenModelPrefab;

    public bool hasLanternInInventory = false;
    public int shovelsInInventory = 0;
    public ToolKind ActiveKind { get; private set; } = ToolKind.None;

    GameObject _activeHandGO;
    LanternHand _lanternHand;
    ShovelHand _shovelHand;

    public bool HasAnyToolEquipped => ActiveKind != ToolKind.None;

    public void AddLanternToInventory() { hasLanternInInventory = true; }
    public void AddShovelToInventory()
    {
        shovelsInInventory = Mathf.Clamp(shovelsInInventory + 1, 0, shovelStockAtShack);
    }

    public bool CanEquip(ToolKind kind, bool isHoldingPickupable)
    {
        if (isHoldingPickupable) return false;
        if (kind == ToolKind.Lantern) return hasLanternInInventory;
        if (kind == ToolKind.Shovel) return shovelsInInventory > 0;
        return false;
    }

    public void Equip(ToolKind kind)
    {
        if (ActiveKind == kind) return;
        Unequip();
        switch (kind)
        {
            case ToolKind.Lantern:
                if (!hasLanternInInventory) return;
                SpawnHand(lanternHandPrefab);
                _lanternHand = _activeHandGO?.GetComponent<LanternHand>();
                ActiveKind = ToolKind.Lantern;
                break;
            case ToolKind.Shovel:
                if (shovelsInInventory <= 0) return;
                SpawnHand(shovelHandPrefab);
                _shovelHand = _activeHandGO?.GetComponent<ShovelHand>();
                ActiveKind = ToolKind.Shovel;
                break;
        }
    }

    public void Unequip()
    {
        ActiveKind = ToolKind.None;
        _lanternHand = null;
        _shovelHand = null;
        if (_activeHandGO) Destroy(_activeHandGO);
        _activeHandGO = null;
    }

    void SpawnHand(GameObject prefab)
    {
        if (!prefab || !handSlot) return;
        _activeHandGO = Instantiate(prefab, handSlot);
        _activeHandGO.transform.localPosition = Vector3.zero;
        _activeHandGO.transform.localRotation = Quaternion.identity;
    }

    public void AutoStowActiveTool() { Unequip(); }

    public bool LanternUsable => _lanternHand && _lanternHand.IsUsable;
    public void RefillLantern(float amount) { _lanternHand?.Refill(amount); }
    public void ThrowLantern(Vector3 dir)
    {
        if (!_lanternHand) return;
        _lanternHand.Throw(dir, lanternBrokenModelPrefab);
        hasLanternInInventory = false;
        Unequip();
    }

    public bool IsDigging => _shovelHand && _shovelHand.IsDigging;
    public bool CanStartDig => ActiveKind == ToolKind.Shovel && _shovelHand && !_shovelHand.IsDigging;
    public void StartDig(System.Action onBroken)
    {
        if (!CanStartDig) return;
        _shovelHand.BeginDig(onBroken);
    }
}