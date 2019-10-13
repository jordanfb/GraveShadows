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
        _defaultShader = Shader.Find("Custom/ItemGlow");
        _glowShader = _renderer.materials[0].shader;

        _renderer.materials[0].shader = _defaultShader;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        _renderer.materials[0].shader = _glowShader;
    }

    private void OnMouseExit()
    {
        _renderer.materials[0].shader = _defaultShader;
    }
}
