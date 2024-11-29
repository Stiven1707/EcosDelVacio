using UnityEngine;

public class SCR_NAR_LUCES_PARPADEO: MonoBehaviour
{
    public Light emergencyLight;
    public float blinkSpeed = 1.0f;

    private void Start()
    {
        if (emergencyLight == null)
            emergencyLight = GetComponent<Light>();
    }

    private void Update()
    {
        // Controla el parpadeo de la luz
        emergencyLight.enabled = Mathf.Sin(Time.time * blinkSpeed) > 0;
    }
}