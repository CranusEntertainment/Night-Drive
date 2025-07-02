using UnityEngine;
using System.Collections.Generic;

public class OptimizedTreeSpawner : MonoBehaviour
{
    [Header("A�a� Ayarlar�")]
    public GameObject treePrefab;
    public int poolSize = 100; // maksimum ka� a�a� spawn edilecek
    public float spacing = 10f;
    public float startZ = 0f;
    public float endZ = 200f;
    public float sideOffset = 5f;

    private Queue<GameObject> treePool;

    void Start()
    {
        InitializePool();

        for (float z = startZ; z <= endZ; z += spacing)
        {
            SpawnTree(new Vector3(-sideOffset, 0, z)); // sol
            SpawnTree(new Vector3(sideOffset, 0, z));  // sa�
        }
    }

    void InitializePool()
    {
        treePool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject tree = Instantiate(treePrefab);
            tree.SetActive(false);
            treePool.Enqueue(tree);
        }
    }

    void SpawnTree(Vector3 position)
    {
        if (treePool.Count == 0)
        {
            Debug.LogWarning("A�a� havuzu bitti! Pool size'� artt�r.");
            return;
        }

        GameObject tree = treePool.Dequeue();
        tree.transform.position = position;
        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        tree.SetActive(true);
    }
}
