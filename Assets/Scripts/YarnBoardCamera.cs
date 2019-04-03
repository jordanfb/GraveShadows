using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnBoardCamera : MonoBehaviour
{

    public float scrollSpeed = 1;
    public float moveSpeed = 1;


    Vector3 startPos;
    public float duration = 0.5f;
    bool lookingAtEvidence = false;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float dzoom = Input.mouseScrollDelta.y * scrollSpeed;
        transform.position += transform.forward * dzoom;
        if (!lookingAtEvidence)
        {
            // move around the world with middle mouse down
            if (Input.GetMouseButton(2))
            {
                // middle mouse down
                transform.position += (transform.right * Input.GetAxis("Mouse X") + transform.up * Input.GetAxis("Mouse Y")) * moveSpeed;
            }
            startPos = transform.position; // set the zoom of the start pos probably?
        }
    }

    public void LookAtEvidence(Transform target)
    {
        Vector3 endPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        lookingAtEvidence = true;
        StartCoroutine(CameraLerp(transform.position, endPos));
    }

    public void ReturnToStart()
    {
        lookingAtEvidence = false;
        StartCoroutine(CameraLerp(transform.position, startPos));
    }

    IEnumerator CameraLerp(Vector3 start, Vector3 end)
    {
        for(float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(start, end, Smootherstep(t / duration));
            yield return 0;
        }

        transform.position = end;
    }

    float Smootherstep(float x)
    {
        // adapted from wikipedia
        return x * x * x * (x * (x * 6 - 15) + 10);
    }
}
