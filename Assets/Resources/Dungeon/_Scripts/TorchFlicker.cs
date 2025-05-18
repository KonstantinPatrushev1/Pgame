using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchFlicker : MonoBehaviour
{
    public Light2D torchLight; // Ссылка на компонент Light2D
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 5f;

    private void Update()
    {
        // Плавное изменение интенсивности света
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
        torchLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}