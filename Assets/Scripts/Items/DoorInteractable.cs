using UnityEngine;

[AddComponentMenu("Interaction/DoorInteractable")]
public class DoorInteractable : MonoBehaviour, IInteractable
{
    public string prompt = "Open/Close";
    public Transform pivot;
    public Vector3 closedEuler;
    public Vector3 openEuler = new Vector3(0, 90, 0);
    public float lerpSpeed = 6f;

    bool _open;

    void Reset()
    {
        pivot = transform;
        closedEuler = transform.localEulerAngles;
    }

    public string GetPrompt(PlayerInteractor interactor) => prompt;
    public bool CanInteract(PlayerInteractor interactor) => true;

    public void Interact(PlayerInteractor interactor)
    {
        _open = !_open;
        StopAllCoroutines();
        StartCoroutine(SmoothRotate(_open ? openEuler : closedEuler));
    }

    System.Collections.IEnumerator SmoothRotate(Vector3 targetEuler)
    {
        var t = 0f;
        var start = pivot.localRotation;
        var end = Quaternion.Euler(targetEuler);
        while (t < 1f)
        {
            t += Time.deltaTime * lerpSpeed;
            pivot.localRotation = Quaternion.Slerp(start, end, t);
            yield return null;
        }

        pivot.localRotation = end;
    }
}