using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LozowichEffect : MonoBehaviour {

    public float intensity;
    public float tileAmount = 1f;
    public float mitigationAmount = 50f;
    private Material material;
    public Texture2DArray test;
    // Creates a private material used to the effect
    
    void Awake ()
    {
        material = new Material( Shader.Find("Hidden/LozowichEffectShader") );
        material.set
    }

    private void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
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
        material.SetFloat("_mitigationAmount", Mathf.Clamp(mitigationAmount, 0f, 100f));
        Graphics.Blit (source, destination, material);
    }
}