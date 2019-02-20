using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    Shader _defaultShader;
    Shader _glowShader;
    // Start is called before the first frame update
    void Start()
    {
        _defaultShader = GetComponent<Renderer>().material.shader;
        _glowShader = Shader.Find("Custom/ItemGlow");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        GetComponent<Renderer>().material.shader = _glowShader;
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.shader = _defaultShader;
    }
}
