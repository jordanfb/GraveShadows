using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    private LineRenderer _line;
    private Vector3 _mousePos;
    private GameObject _startPin;
    private int _currentLines = 0;

    public Material material;
    public float _lineWidth = 0.15f;

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
        //BUG: Need to store start pin so yarn doesn't connect with itself
        else if(Input.GetMouseButtonUp(0) && _line)
        {
            //Cast a ray again from mouse to screen
            RaycastHit endHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out endHit))
            {
                Collider endColl = endHit.collider;
                if(endColl.gameObject.tag == "Pin" && endColl.gameObject != _startPin)
                {
                    //Set the endpoint position of the line to the end pin
                    _line.SetPosition(1, endColl.gameObject.transform.position);
                    _currentLines++;
                }
                else
                {
                    //Line is invalid, destroy
                    Destroy(_line.gameObject);
                }
            }
            _line = null;
            _startPin = null;
        }
        //An attempt at dragging the line, but without a raycast it doesn't really work
        /*
        else if(Input.GetMouseButton(0) && _line != null) 
        {
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePos.z = _line.GetPosition(0).z;

            _line.SetPosition(1, _mousePos);
        }*/

        //Delete a line on right click
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit delHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out delHit))
            {

            }
        }
    }

    private void CreateLine(Vector3 position)
    {
        if (_line != null) return;
        _line = new GameObject("Line" + _currentLines).AddComponent<LineRenderer>();
        _line.material = material;
        _line.positionCount = 2;
        _line.startWidth = _lineWidth;
        _line.useWorldSpace = true;
        _line.SetPosition(0, position);
        _line.SetPosition(1, position);
    }

    //Need to create a box collider around each line, otherwise they cannot
    //be removed using right click.
    private void AddCollider(LineRenderer line)
    {

    }
}
