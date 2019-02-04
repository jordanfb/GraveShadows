using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evidence : MonoBehaviour
{
    Shader _defaultShader;

    private void Start()
    {
        _defaultShader = GetComponent<Renderer>().material.shader;
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
