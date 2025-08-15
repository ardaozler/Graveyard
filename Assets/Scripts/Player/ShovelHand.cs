using UnityEngine;
using System.Collections;

public class ShovelHand : MonoBehaviour
{
    public Animator animator;
    public float digDuration = 2.0f;
    public float breakChancePerDig = 0.0f;
    public bool IsDigging { get; private set; }

    public void BeginDig(System.Action onBroken)
    {
        if (IsDigging) return;
        StartCoroutine(DigRoutine(onBroken));
    }

    IEnumerator DigRoutine(System.Action onBroken)
    {
        IsDigging = true;
        animator?.SetTrigger("Dig");
        yield return new WaitForSeconds(digDuration);

        if (breakChancePerDig > 0f && Random.value < breakChancePerDig)
        {
            onBroken?.Invoke();
        }
        IsDigging = false;
    }
}