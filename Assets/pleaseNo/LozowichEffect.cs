using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LozowichEffect : MonoBehaviour {

    public float intensity;
    public float tileAmount = 1f;
    private Material material;

    // Creates a private material used to the effect
    void Awake ()
    {
        material = new Material( Shader.Find("Hidden/LozowichEffectShader") );
    }
    
    // Postprocess the image
    void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        if (intensity == 0)
        {
            Graphics.Blit (source, destination);
            return;
        }

        material.SetFloat("_bwBlend", Mathf.Clamp(intensity,0f, 1f)); 
        material.SetFloat("_tile", Mathf.Clamp(tileAmount, 0f, 10f));
        Graphics.Blit (source, destination, material);
    }
}