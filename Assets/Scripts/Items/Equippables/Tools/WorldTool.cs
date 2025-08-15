using System;
using UnityEngine;

[AddComponentMenu("Items/WorldTool (Collect to Inventory)")]
public class WorldTool : MonoBehaviour, IInteractable
{
    public ToolKind kind = ToolKind.Lantern;
    public GameObject brokenModelPrefab;
    public bool destroyOnCollect = true;

    public string GetPrompt(PlayerInteractor interactor)
    {
        return kind switch
        {
            ToolKind.Lantern => "Collect Lantern",
            ToolKind.Shovel => "Collect Shovel"
        };
    }

    public bool CanInteract(PlayerInteractor interactor)
    {
        return interactor && interactor.Inventory != null;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!CanInteract(interactor)) return;
        var inv = interactor.Inventory;
        switch (kind)
        {
            case ToolKind.Lantern:
                inv.AddLanternToInventory();
                break;
            case ToolKind.Shovel:
                inv.AddShovelToInventory();
                break;
        }
        if (destroyOnCollect) Destroy(gameObject); else gameObject.SetActive(false);
    }

    public static void SpawnBrokenModel(GameObject brokenPrefab, Vector3 pos, Quaternion rot)
    {
        if (brokenPrefab) Instantiate(brokenPrefab, pos, rot);
    }
}