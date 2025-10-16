using System.Collections;
using UnityEngine;

public class PondSpawning : MonoBehaviour
{
    [SerializeField] public GameObject Food;
    GameObject test;
    bool Spawning = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
        
    IEnumerator Spawn()
    {
        Spawning = true;
        yield return new WaitForSeconds(5);
        CreateStuffOnRoad();
        Spawning = false;
    }
    void CreateStuffOnRoad()
    {
        Vector3 randomizePosition = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));

        Quaternion randomizeRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        test = Instantiate(Food, randomizePosition, randomizeRotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Spawning)
        {
            StartCoroutine(Spawn());
        }
    }
}
