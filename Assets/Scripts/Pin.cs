using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    Shader _defaultShader;
    // Start is called before the first frame update
    void Start()
    {
        _defaultShader = GetComponent<Renderer>().material.shader;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        GetComponent<Renderer>().material.shader = Shader.Find("Custom/ItemGlow");
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.shader = _defaultShader;
    }
}
