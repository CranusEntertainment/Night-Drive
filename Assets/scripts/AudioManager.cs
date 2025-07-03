using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Havuz Ayarlarý")]
    public int poolSize = 5;

    [Header("Ses Çalma Ayarlarý")]
    public float minInterval = 0.2f; // Ayný sesler arasý minimum bekleme süresi (saniye)

    private List<AudioSource> sources;
    private int currentSourceIndex = 0;
    private float lastPlayTime = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sources = new List<AudioSource>();

            for (int i = 0; i < poolSize; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = false;           // Döngü kapalý olmalý
                source.spatialBlend = 0f;      // 2D ses
                source.priority = 128;         // Orta öncelik (0 en yüksek, 256 en düþük)
                sources.Add(source);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        // Çok sýk ses çalýnmasýný engelle
        if (Time.time - lastPlayTime < minInterval)
        {
            Debug.Log("Ses çok sýk çalýyor, atlandý.");
            return;
        }

        AudioSource source = null;

        // Round Robin: sýradaki AudioSource'u seç
        for (int i = 0; i < poolSize; i++)
        {
            int index = (currentSourceIndex + i) % poolSize;
            if (!sources[index].isPlaying)
            {
                source = sources[index];
                currentSourceIndex = (index + 1) % poolSize;
                break;
            }
        }

        if (source != null)
        {
            source.clip = clip;
            source.Play();
            lastPlayTime = Time.time;
            Debug.Log("Ses çalýyor: " + clip.name);
        }
        else
        {
            Debug.LogWarning("Ses kanalý bulunamadý, ses atlandý: " + clip.name);
        }
    }
}
