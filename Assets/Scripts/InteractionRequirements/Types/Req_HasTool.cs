using UnityEngine;

[CreateAssetMenu(menuName = "Interaction/Requirement/Has Tool", fileName = "Req_HasTool_")]
public class Req_HasTool : InteractionRequirement
{
    public ToolDefinition requiredTool;

    public override bool IsSatisfied(PlayerInteractor interactor)
    {
        if (!interactor || !requiredTool) return false;
        var eq = interactor.GetComponent<PlayerEquipment>();
        return eq && eq.HasTool(requiredTool);
    }

    public override string GetBlockedReason(PlayerInteractor interactor)
    {
        return requiredTool ? $"Requires {requiredTool.displayName}" : "Requires Tool";
    }
}