using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnLine : MonoBehaviour
{

    // for undoing/redoing this yarn collider:
    public SerializedEvidence e1;
    public SerializedEvidence e2;

    // for making the yarn itself
    public Transform point1;
    public Transform point2;
    //public Vector3 tempPoint;
    public bool lockToPoints = false;
    public float colliderSizeScalar = 2f;

    public LineRenderer _line;
    BoxCollider lineColl;

    // Start is called before the first frame update
    void Start()
    {
        _line = GetComponent<LineRenderer>();
        lineColl = GetComponentInChildren<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lockToPoints && (point1.position != _line.GetPosition(0) || point2.position != _line.GetPosition(1)))
        {
            _line.SetPosition(0, point1.position);
            _line.SetPosition(1, point2.position);
            RefreshCollider();
        }
    }

    private void RefreshCollider()
    {
        //Calculate the length and width of the line to determine size of box
        float lineWidth = _line.startWidth * colliderSizeScalar;
        float lineLength = Vector3.Distance(_line.GetPosition(0), _line.GetPosition(1));

        //Z-size is arbitrary, and we might need to change these around
        //depending on what axis the yarn board faces
        lineColl.size = new Vector3(lineLength, lineWidth, lineWidth);

        //Tne line collider GO will lie at the midpoint of the line to cover  the whole thing.
        Vector3 midPoint = (_line.GetPosition(0) + _line.GetPosition(1)) / 2;
        lineColl.transform.position = midPoint;

        //Rotate the line collider by a certain angle so that it's oriented correctly.
        float angle = Mathf.Atan2((_line.GetPosition(1).y - _line.GetPosition(0).y), (_line.GetPosition(1).x - _line.GetPosition(0).x));
        angle *= Mathf.Rad2Deg;
        lineColl.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
