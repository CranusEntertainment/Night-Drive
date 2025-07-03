using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI; // En üstte ekle
using UnityEngine.SceneManagement; // Sahne yönetimi için ekle


public class CarMovement : MonoBehaviour
{
    [Header("İleri Gitme (Z ekseni azalacak)")]
    public float forwardSpeed = 10f;
    public float boostedSpeed = 20f;
    public float boostLerpSpeed = 5f;
    public float currentSpeed;

    [Header("Küre Toplama Ayarları")]
    public float speedIncreasePerOrb = 1f;
    public float maxForwardSpeed = 25f;

    [Header("Hız Düşürme Ayarları")]
    public float speedDecreaseAmount = 5f;
    public float minForwardSpeed = 5f; // Hızın düşebileceği minimum değer

    [Header("UI referans")]
    public Text speedDisplayText;
    public GameObject gameOverPanel;  // Buraya inspector'dan GameOverPanel'i bağlayacaksın

    [Header("Sağa–Sola Hareket")]
    public float sideSpeed = 5f;
    public float maxX = 4f;
    public float minX = -4f;

    [Header("Yatış Efekti")]
    public float tiltAmount = 10f;
    public float tiltSpeed = 5f;

    [Header("Post-Processing Ayarları")]
    public Volume globalVolume;
    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;
    private MotionBlur motionBlur;

    [Header("Ses Ayarları")]
    public AudioSource engineAudioSource;
    public float minPitch = 0.8f;
    public float maxPitch = 1.5f;

    [Header("Trail Ayarları")]
    public TrailRenderer rightBoostTrail;
    public TrailRenderer leftBoostTrail;
    public float defaultTrailTime = 0.2f;
    public float boostedTrailTime = 1.5f;
    public float trailLerpSpeed = 5f;

    private int moveDirection = 0;
    private bool isBoosting = false;
    private bool isGameOver = false;

    void Start()
    {
        currentSpeed = forwardSpeed;

        if (globalVolume != null)
        {
            globalVolume.profile.TryGet(out lensDistortion);
            globalVolume.profile.TryGet(out chromaticAberration);
            globalVolume.profile.TryGet(out motionBlur);

            if (lensDistortion != null) lensDistortion.intensity.value = 0f;
            if (motionBlur != null) motionBlur.intensity.value = 0f;
            if (chromaticAberration != null) chromaticAberration.intensity.value = 0f;
        }
        // Başlangıçta GameOverPanel gizli olsun
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (engineAudioSource != null)
            engineAudioSource.Play();

        if (rightBoostTrail != null)
            rightBoostTrail.time = defaultTrailTime;
        if (leftBoostTrail != null)
            leftBoostTrail.time = defaultTrailTime;

        Time.timeScale = 1f; // Oyun başladığında zaman akışını resetle
    }

    void Update()
    {
        float delta = Time.deltaTime;
        // Update içinde speedRatio hesaplama
        float speedRatio = Mathf.InverseLerp(0f, maxForwardSpeed, currentSpeed);
        float targetSpeed = isBoosting ? boostedSpeed : forwardSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, boostLerpSpeed * delta);

        // Speed sıfıra inmiş mi kontrolü
        if (currentSpeed <= 0.01f) // sıfıra çok yakınsa game over
        {
            currentSpeed = 0f;
            GameOver();
            return;
        }


        // Text güncellemesi
        if (speedDisplayText != null)
            speedDisplayText.text = "KM: " + Mathf.RoundToInt(currentSpeed).ToString();


        // Motor sesi
        if (engineAudioSource != null)
            engineAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);

        // Hareket
        Vector3 pos = transform.position;
        pos.z -= currentSpeed * delta;

        if (moveDirection != 0)
        {
            pos.x += moveDirection * sideSpeed * delta;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
        }
        transform.position = pos;

        // Görsel efektler
        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Lerp(0f, -0.5f, speedRatio);

        if (motionBlur != null)
            motionBlur.intensity.value = Mathf.Lerp(0f, 0.5f, speedRatio);

        // Yatış efekti
        float targetZRotation = moveDirection == 1 ? -tiltAmount : moveDirection == -1 ? tiltAmount : 0f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, tiltSpeed * delta);

        // Trail boylarını boost durumuna göre güncelle
        float targetTrailTime = isBoosting ? boostedTrailTime : defaultTrailTime;

        if (rightBoostTrail != null)
            rightBoostTrail.time = Mathf.Lerp(rightBoostTrail.time, targetTrailTime, trailLerpSpeed * delta);

        if (leftBoostTrail != null)
            leftBoostTrail.time = Mathf.Lerp(leftBoostTrail.time, targetTrailTime, trailLerpSpeed * delta);
    }


    // === Yeni Eklenen Metod ===
    public void CollectOrb()
    {
        forwardSpeed += speedIncreasePerOrb;
        boostedSpeed += speedIncreasePerOrb;

        forwardSpeed = Mathf.Min(forwardSpeed, maxForwardSpeed);
        boostedSpeed = Mathf.Min(boostedSpeed, maxForwardSpeed);
    }

    void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Oyun bitti! Hız 0'a düştü.");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true); // Game Over paneli göster

        Time.timeScale = 0f; // Oyunu durdur
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Eğer oyunu durdurduysan aç
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }


    // Yeni: Hızı düşüren metod
    public void ApplySlowDown()
    {
        forwardSpeed -= speedDecreaseAmount;
        boostedSpeed -= speedDecreaseAmount;

        forwardSpeed = Mathf.Max(forwardSpeed, 0f); // minForwardSpeed yerine 0
        boostedSpeed = Mathf.Max(boostedSpeed, 0f);

        Debug.Log("Hız düşürüldü! Yeni hız: " + forwardSpeed);
    }

    // UI butonları
    public void MoveRightStart() => moveDirection = 1;
    public void MoveLeftStart() => moveDirection = -1;
    public void StopMove() => moveDirection = 0;
    public void StartBoost() => isBoosting = true;
    public void StopBoost() => isBoosting = false;
}
