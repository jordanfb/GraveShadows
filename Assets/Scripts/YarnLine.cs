using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnLine : MonoBehaviour
{

    public Transform point1;
    public Transform point2;
    //public Vector3 tempPoint;
    public bool lockToPoints = false;

    public LineRenderer _line;

    // Start is called before the first frame update
    void Start()
    {
        _line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lockToPoints)
        {
            _line.SetPosition(0, point1.position);
            _line.SetPosition(1, point2.position);
        }
    }
}
