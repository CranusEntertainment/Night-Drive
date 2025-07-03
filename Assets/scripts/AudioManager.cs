using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Havuz Ayarlar�")]
    public int poolSize = 5;

    [Header("Ses �alma Ayarlar�")]
    public float minInterval = 0.2f; // Ayn� sesler aras� minimum bekleme s�resi (saniye)

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
                source.loop = false;           // D�ng� kapal� olmal�
                source.spatialBlend = 0f;      // 2D ses
                source.priority = 128;         // Orta �ncelik (0 en y�ksek, 256 en d���k)
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

        // �ok s�k ses �al�nmas�n� engelle
        if (Time.time - lastPlayTime < minInterval)
        {
            Debug.Log("Ses �ok s�k �al�yor, atland�.");
            return;
        }

        AudioSource source = null;

        // Round Robin: s�radaki AudioSource'u se�
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
            Debug.Log("Ses �al�yor: " + clip.name);
        }
        else
        {
            Debug.LogWarning("Ses kanal� bulunamad�, ses atland�: " + clip.name);
        }
    }
}
