using UnityEngine;
using UnityEngine.Events;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private ToolDefinition currentTool;

    [System.Serializable] public class ToolChangedEvent : UnityEvent<ToolDefinition> {}
    public ToolChangedEvent onEquippedChanged;

    public ToolDefinition CurrentTool => currentTool;
    
    public bool HasTool(ToolDefinition tool)
    {
        return tool != null && currentTool == tool;
    }

    public void Equip(ToolDefinition tool)
    {
        if (currentTool == tool) return;
        currentTool = tool;
        onEquippedChanged?.Invoke(currentTool);
    }

    public void Unequip()
    {
        if (currentTool == null) return;
        currentTool = null;
        onEquippedChanged?.Invoke(currentTool);
    }
}