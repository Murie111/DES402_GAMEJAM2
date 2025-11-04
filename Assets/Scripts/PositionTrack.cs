using UnityEngine;

public class PositionTrack : MonoBehaviour
{
    public float boundsRange;
    public Vector3 boundsCentre;

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(boundsCentre, boundsRange);
    }
}
