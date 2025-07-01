using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight;  // Directional light referans�
    public float dayDuration = 24f; // Bir g�n�n s�resi (saniye cinsinden)
    private float timeOfDay = 0f;   // G�n�n saati (0-24 aras�)

    void Update()
    {
        // Zaman� ilerlet
        timeOfDay += Time.deltaTime / dayDuration * 24f;  // 24 saatlik d�ng�
        if (timeOfDay >= 24f)
        {
            timeOfDay = 0f;  // Zaman 24 saatlik d�ng�y� ge�ti�inde s�f�rla
        }

        // Directional light'�n d�n���n� ayarla
        float rotationAngle = timeOfDay / 24f * 360f;  // 24 saatte 360 derece d�nmeli
        directionalLight.transform.rotation = Quaternion.Euler(rotationAngle - 18.872f, 0f, 0f);  // D�n�� a��s�n� ayarla
    }
}
