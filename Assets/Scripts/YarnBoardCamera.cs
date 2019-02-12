using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnBoardCamera : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;
    public float duration = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LookAtEvidence(Transform target)
    {
        Vector3 endPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        StartCoroutine(CameraLerp(transform.position, endPos));
        
    }

    IEnumerator CameraLerp(Vector3 start, Vector3 end)
    {
        for(float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(start, end, t / duration);
            yield return 0;
        }

        transform.position = end;
    }
}
