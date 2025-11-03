using System.Collections;
using UnityEngine;

public class Food : MonoBehaviour
{
    public GameObject gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(30);
        Destroy(gameObject);
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Mouth"))
        {
            gameManager.GetComponent<GlobalManager>().increaseFishPoints();
            Destroy(gameObject);
        }
    }
}
