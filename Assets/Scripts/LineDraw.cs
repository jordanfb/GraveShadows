using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    private LineRenderer _line;
    private Vector3 _mousePos;
    private GameObject _startPin;
    private List<LineRenderer> _lines;

    public Material material;
    public float _lineWidth = 0.15f;

    private void Start()
    {
        _lines = new List<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Left click creates a new line if clicking a pin.
        if(Input.GetMouseButtonDown(0))
        {
            if(_line == null)
            {
                //Cast a ray from the mouse to the screen
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //If it's a hit, check to see if the gameobject is tagged as a pin
                if(Physics.Raycast(ray, out hit))
                {
                    Collider coll = hit.collider;
                    if(coll.gameObject.tag == "Pin")
                    {
                        //Create a line if it's a pin object
                        _startPin = coll.gameObject;
                        CreateLine(coll.gameObject.transform.position);
                    }
                }
            }

        }
        //Once the held mouse button is released
        else if(Input.GetMouseButtonUp(0) && _line)
        {
            //Cast a ray again from mouse to screen
            RaycastHit endHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out endHit))
            {
                Collider endColl = endHit.collider;
                if(endColl.gameObject.tag == "Pin" && 
                    endColl.gameObject != _startPin &&
                    !ConnectionExists(endColl.gameObject.transform.position))
                {
                    //Set the endpoint position of the line to the end pin
                    _line.SetPosition(1, endColl.gameObject.transform.position);
                    AddCollider();
                    _lines.Add(_line);
                }
                else
                {
                    //Line is invalid, destroy
                    _lines.Remove(_line);
                    Destroy(_line.gameObject);
                }
            }
            _line = null;
            _startPin = null;
        }
        //An attempt at dragging the line, but without a raycast it doesn't really work
        
        else if(Input.GetMouseButton(0) && _line != null) 
        {
            Debug.Log("HERE");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit endHit;
            if (Physics.Raycast(ray, out endHit, Mathf.Infinity, LayerMask.GetMask("YarnBoard")))
            {
                Debug.Log("HITHITHIT");
                Vector3 pos = endHit.point;
                pos.z = _line.GetPosition(0).z;
                // this is a bit of an issue since it means it doesn't perfectly align with
                // the mouse but it does mean it stays above the evidence. It doesn't work for rotated yarn boards though

                // this method is slightly better maybe but still not great
                // pos += endHit.normal; // move it up by 1 so it's above the evidence while still allowing a tilted yarn board

                _line.SetPosition(1, pos);
            }
        }

        //Delete a line on right click
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit delHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out delHit))
            {
                //If we hit a line collider, destroy the parent
                //(which in turn destroys all children and thus the collider is deleted too).
                Collider delColl = delHit.collider;
                if(delColl.gameObject.tag == "Line")
                {
                    LineRenderer delLine = delColl.transform.parent.gameObject.GetComponent<LineRenderer>();
                    _lines.Remove(delLine);
                    Destroy(delColl.transform.parent.gameObject);
                }
            }
        }

    }

    /**
     * Creates a line with vertices at the transform of the starting pin.
     * Requires line to be null, otherwise you could take control of multiple lines at once.
     */
    private void CreateLine(Vector3 position) // Can probably remove the vector3 arg now that I track the starting pin
    {
        if (_line != null) return;
        _line = new GameObject("Line" + _lines.Count).AddComponent<LineRenderer>();
        //Eventually this material will be an actual yarn texture
        _line.material = material;
        _line.positionCount = 2;
        _line.startWidth = _lineWidth;
        _line.useWorldSpace = true;

        //Set both vertices to the start pin
        _line.SetPosition(0, position);
        _line.SetPosition(1, position);
    }

    /**
     * Creates a GameObject with a box collider that wraps around a
     * newly connected line.
     * */
    private void AddCollider()
    {
        //Create a new gameobject with a box collider
        BoxCollider lineColl = new GameObject("LineCollider" + _lines.Count).AddComponent<BoxCollider>();
        lineColl.transform.parent = _line.transform;
        //Tag required for the raycast check (right clicking to delete)
        lineColl.gameObject.tag = "Line";

        //Calculate the length and width of the line to determine size of box
        float lineWidth = _line.startWidth;
        float lineLength = Vector3.Distance(_line.GetPosition(0), _line.GetPosition(1));

        //Z-size is arbitrary, and we might need to change these around
        //depending on what axis the yarn board faces
        lineColl.size = new Vector3(lineLength, lineWidth, 1f);

        //Tne line collider GO will lie at the midpoint of the line to cover  the whole thing.
        Vector3 midPoint = (_line.GetPosition(0) + _line.GetPosition(1)) / 2;
        lineColl.transform.position = midPoint;

        //Rotate the line collider by a certain angle so that it's oriented correctly.
        float angle = Mathf.Atan2((_line.GetPosition(1).y - _line.GetPosition(0).y), (_line.GetPosition(1).x - _line.GetPosition(0).x));
        angle *= Mathf.Rad2Deg;
        lineColl.transform.Rotate(0f, 0f, angle);
    }

    /**
     * Returns true if and only if a line exists with the exact
     * endpoint positions. Returns false otherwise.
     * */
    public bool ConnectionExists(Vector3 endPos)
    {
        foreach(LineRenderer li in _lines)
        {
            if (li.GetPosition(0) == _startPin.transform.position &&
                li.GetPosition(1) == endPos)
                return true;
            if (li.GetPosition(1) == _startPin.transform.position &&
                li.GetPosition(0) == endPos)
                return true;
        }
        return false;
    }
}
