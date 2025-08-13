using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    public PlayerInteractor interactor;   
    public TextMeshProUGUI label;         

    void LateUpdate()
    {
        if (!interactor || !label) return;

        string prompt = null;
        var cam = Camera.main;
        if (cam && Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactor.interactDistance, interactor.interactMask))
        {
            var i = hit.collider.GetComponentInParent<IInteractable>();
            if (i != null && i.CanInteract(interactor))
                prompt = i.GetPrompt(interactor);
            else
            {
                var p = hit.collider.GetComponentInParent<Pickupable>();
                if (p != null && !p.IsHeld)
                    prompt = "Pick Up";
            }
        }

        label.gameObject.SetActive(!string.IsNullOrEmpty(prompt));
        if (!string.IsNullOrEmpty(prompt))
            label.text = $"[E] {prompt}";
    }
}