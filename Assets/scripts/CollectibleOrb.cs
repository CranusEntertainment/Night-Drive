using UnityEngine;

public class CollectibleOrb : MonoBehaviour
{
    public GameObject collectEffect;       // Opsiyonel: particle efekti
    public AudioClip collectSound;         // SFX: Ses klibi

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarMovement car = other.GetComponent<CarMovement>();
            if (car != null)
            {
                car.CollectOrb();

                Debug.Log("K�re topland�! H�z art�r�ld�.");

                if (collectSound != null)
                    AudioManager.Instance.PlaySFX(collectSound);


                // Efekt g�ster
                if (collectEffect != null)
                    Instantiate(collectEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
    }
}
