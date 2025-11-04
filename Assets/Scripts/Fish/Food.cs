using System.Collections;
using UnityEngine;

public class Food : MonoBehaviour
{
    bool growing;
    bool shrinking;
    float currentLimit;
    bool isBonus = false;
    public GameObject gameManager;

    private BoxCollider bc;
    //[SerializeField] private SpriteRenderer bug;
    [SerializeField] private MeshRenderer mr;
    [SerializeField] private Anims biteThing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        StartCoroutine(Death());

        bc = GetComponent<BoxCollider>();
        
    }

    private void Awake()
    {
        int randomCheck = Random.Range(0, 11);
        Debug.Log(randomCheck);
        if (randomCheck == 10)
        {
            Debug.Log("bonus!");
            currentLimit = 2;
            growing = true;
        }
        else
        {
            currentLimit = 1;
            growing = true;
        }
    }
    private void FixedUpdate()
    {
        if (growing)
        {
            transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime, transform.localScale.y + Time.deltaTime, transform.localScale.z + Time.deltaTime);
            if (transform.localScale.x >= currentLimit)
            {
                growing = false;
            }
        }

        if (shrinking)
        {
            transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime, transform.localScale.y - Time.deltaTime, transform.localScale.z - Time.deltaTime);
            if (transform.localScale.x <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(25);
        shrinking = true;
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Mouth"))
        {
            if (currentLimit == 2)
            {
                gameManager.GetComponent<GlobalManager>().increaseFishPointsBonus();
            }
            else
            {
                gameManager.GetComponent<GlobalManager>().increaseFishPoints();

            }

            bc.enabled = false;
            mr.enabled= false;
            //bug.enabled = false;

            biteThing.PlayAnim(Random.Range(0, 3));
            Invoke("destroyFood", 1.5f);
        }
    }

    void destroyFood()
    {
        Destroy(gameObject);
    }
}
