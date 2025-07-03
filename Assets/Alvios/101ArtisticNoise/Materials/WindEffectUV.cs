using UnityEngine;
using UnityEngine.UI;

public class WindEffectUV : MonoBehaviour
{
    public RawImage rawImage; // UI kullanýyorsan
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.0f;

    private void Update()
    {
        Rect uvRect = rawImage.uvRect;
        uvRect.x += scrollSpeedX * Time.deltaTime;
        uvRect.y += scrollSpeedY * Time.deltaTime;
        rawImage.uvRect = uvRect;
    }
}
