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
}