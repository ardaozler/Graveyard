using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    public void ChangeColorRandom()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = new Color(Random.value, Random.value, Random.value);
    }

    public void PrintText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("Text is null or empty.");
            return;
        }

        Debug.Log(text);
    }

    public void DropPositionBy(float offset)
    {
        Transform transform = GetComponent<Transform>();
        if (transform == null)
        {
            Debug.LogWarning("No Transform component found.");
            return;
        }

        Vector3 newPosition = transform.position + Vector3.down * offset;
        transform.position = newPosition;
    }
}