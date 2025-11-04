using UnityEngine;

public class LazyPoint : MonoBehaviour
{
    //[SerializeField] GameObject target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.gameObject.transform.eulerAngles= new Vector3(0,0,0);
    }
}
