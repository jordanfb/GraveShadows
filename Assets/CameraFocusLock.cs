using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusLock : MonoBehaviour
{
    public bool lockX;
    public bool lockY;
    public bool lockZ;

    public Transform toFollow;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = toFollow.position;
        if (lockX)
            pos.x = transform.position.x;
        if (lockY)
            pos.y = transform.position.y;
        if (lockZ)
            pos.z = transform.position.z;
        transform.position = pos;
    }
}
