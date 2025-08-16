using UnityEngine;

public class DoorMover : MonoBehaviour
{
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

    public void ToggleDoorState()
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