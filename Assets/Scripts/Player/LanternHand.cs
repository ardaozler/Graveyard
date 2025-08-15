using UnityEngine;

public class LanternHand : MonoBehaviour
{
    public float maxOil = 100f;
    public float oil = 100f;
    public float drainPerSecond = 1.5f;
    public float minLightIntensity = 0.05f;
    public Light lanternLight;
    public Rigidbody thrownLanternPrefab;

    public bool IsUsable => oil > 0.01f;

    void Update()
    {
        if (oil > 0f) oil = Mathf.Max(0f, oil - drainPerSecond * Time.deltaTime);
        UpdateLight();
    }

    void UpdateLight()
    {
        if (!lanternLight) return;
        float t = (maxOil <= 0f) ? 0f : Mathf.Clamp01(oil / maxOil);
        lanternLight.intensity = Mathf.Lerp(minLightIntensity, 1f, t);
    }

    public void Refill(float amount)
    {
        oil = Mathf.Clamp(oil + Mathf.Abs(amount), 0f, maxOil);
        UpdateLight();
    }

    public void Throw(Vector3 dir, GameObject brokenModelPrefab)
    {
        if (!thrownLanternPrefab) return;
        var rb = Instantiate(thrownLanternPrefab, transform.position, transform.rotation);
        rb.AddForce(dir.normalized * 8f, ForceMode.VelocityChange);
    }
}