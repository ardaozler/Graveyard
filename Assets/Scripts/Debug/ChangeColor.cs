using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public void ChangeColorRandom()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color =  new Color(Random.value, Random.value, Random.value);
    }
}