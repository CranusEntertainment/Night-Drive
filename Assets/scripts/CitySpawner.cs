using System.Collections.Generic;
using UnityEngine;

public class CitySpawner : MonoBehaviour
{
    private Transform player;  // private, editörden görünmez

    public GameObject[] segmentPrefabs;
    public int visibleSegments = 5;
    public float segmentLength = 100f;

    private float spawnZ = 0f;
    private List<GameObject> spawnedSegments = new List<GameObject>();

    private Vector3 spawnPos = Vector3.zero;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player bulunamadı! Player objesine 'Player' tag'i atanmış mı?");
        }

        for (int i = 0; i < visibleSegments; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (player == null)
            {
                Debug.LogWarning("Player bulunamadı, Update atlandı.");
                return;
            }
        }
        float playerZ = player.position.z;

        if (playerZ < spawnZ + (visibleSegments * segmentLength))
        {
            SpawnSegment();
        }

        if (spawnedSegments.Count > 0)
        {
            GameObject firstSegment = spawnedSegments[0];
            float segmentEndZ = firstSegment.transform.position.z - segmentLength;

            if (playerZ < segmentEndZ)
            {
                RemoveOldestSegment();
            }
        }
    }

    void SpawnSegment()
    {
        int index = Random.Range(0, segmentPrefabs.Length);
        GameObject prefabToSpawn = segmentPrefabs[index];

        spawnPos.Set(0f, 0f, spawnZ);
        GameObject segment = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        spawnedSegments.Add(segment);
        spawnZ -= segmentLength;
    }

    void RemoveOldestSegment()
    {
        if (spawnedSegments[0] != null)
        {
            Destroy(spawnedSegments[0]);
        }
        spawnedSegments.RemoveAt(0);
    }
}
    