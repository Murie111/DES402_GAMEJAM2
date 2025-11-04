using UnityEngine;
using System.Collections;

public class fishScale : MonoBehaviour
{

    private void Start()
    {
        Scale();
    }

    public void Scale()
    {
        StartCoroutine(ScaleChange(1f));
    }

    private IEnumerator ScaleChange(float time)
    {
        var t = 0f;

        while (t <= 1f)
        {
            t += Time.deltaTime / time;

            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            yield return new WaitForFixedUpdate();
        }
    }
}
