using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public static FishSpawner Instance;

    public GameObject fishPrefab;
    public int StartingNum;

    public float range;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        for (int i = 0; i < StartingNum; i++)
            SpawnFish();
    }

    void SpawnFish()
    {
        Instantiate(fishPrefab, GetPosition(), Quaternion.identity);
    }

    Vector3 GetPosition()
    {
        Vector3 spawnPos = Random.insideUnitSphere * range + transform.position;

        //this is so stupid
        spawnPos.y = 4f;

        return spawnPos;
    }

    public void ResetFish(GameObject fish)
    {
        fish.transform.position = GetPosition();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
