using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplySlowDown : MonoBehaviour
{
    public GameObject collectEffect;       // Opsiyonel: particle efekti
    public AudioClip collectSound;         // SFX: Ses klibi


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarMovement car = other.GetComponent<CarMovement>();
            if (car != null)
            {
                car.ApplySlowDown();

                Debug.Log("Yavaþlatma uygulandý!");
                if (collectSound != null)
                    AudioManager.Instance.PlaySFX(collectSound);


                // Efekt göster
                if (collectEffect != null)
                    Instantiate(collectEffect, transform.position, Quaternion.identity);




            }
        }
    }

}
