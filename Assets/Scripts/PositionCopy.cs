using UnityEngine;

public class PositionCopy : MonoBehaviour
{
    public PositionTrack trackObject;

    public float boundsRange;
    public Vector3 boundsCentre;

    private float scale;

    void Start()
    {
        scale = boundsRange / trackObject.boundsRange;
    }

    void Update()
    {
        //tracks object if it is within its own range
        if ((trackObject.boundsCentre - trackObject.transform.position).sqrMagnitude < Mathf.Pow(trackObject.boundsRange, 2f))
        {
            //converts tracked postion to local position
            Vector3 trackedLocalPosition = trackObject.transform.position - trackObject.boundsCentre;

            //scales to copied bounds
            Vector3 copiedLocalPosition = (trackedLocalPosition * scale);

            transform.position = new Vector3(boundsCentre.x + copiedLocalPosition.x, transform.position.y, boundsCentre.z + copiedLocalPosition.z);

            transform.rotation = trackObject.transform.rotation;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(boundsCentre, boundsRange);
    }
}
