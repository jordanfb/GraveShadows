using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    private LineRenderer _line;
    private Vector3 _mousePos;
    private int _currentLines = 0;

    public Material material;
    public float zPos;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(_line == null)
            {
                CreateLine();
            }

            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePos.z = zPos;

            _line.SetPosition(0, _mousePos);
            _line.SetPosition(1, _mousePos);
        }
        else if(Input.GetMouseButtonUp(0) && _line)
        {
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePos.z = zPos;

            _line.SetPosition(1, _mousePos);
            Debug.Log(_line.GetPosition(0));
            Debug.Log(_line.GetPosition(1));
            _line = null;
            _currentLines++;
        }
        else if(Input.GetMouseButton(0) && _line) 
        {
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePos.z = zPos;

            _line.SetPosition(1, _mousePos);
        }
    }

    private void CreateLine()
    {
        _line = new GameObject("Line" + _currentLines).AddComponent<LineRenderer>();
        _line.material = material;
        _line.positionCount = 2;
        _line.startWidth = 0.15f;
        _line.useWorldSpace = true;
    }
}
