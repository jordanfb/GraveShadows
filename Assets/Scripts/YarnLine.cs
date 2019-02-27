using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnLine : MonoBehaviour
{

    public Transform point1;
    public Transform point2;
    public Vector3 tempPoint;
    public bool useTempPoint;

    public LineRenderer _line;

    // Start is called before the first frame update
    void Start()
    {
        _line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
