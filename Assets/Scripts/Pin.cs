using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    Shader _defaultShader;
    Shader _glowShader;
    [SerializeField]
    MeshRenderer _renderer;
    // Start is called before the first frame update
    void Start()
    {
        _defaultShader = _renderer.materials[0].shader;
        _glowShader = Shader.Find("Custom/ItemGlow");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        _renderer.materials[0].shader = _glowShader;
    }

    private void OnMouseExit()
    {
        _renderer.materials[0].shader = _defaultShader;
    }
}
