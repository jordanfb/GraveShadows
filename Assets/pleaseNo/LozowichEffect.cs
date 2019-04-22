using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LozowichEffect : MonoBehaviour {

    public float intensity;
    public float tileAmount = 1f;
    public float mitigationAmount = 50f;
    public int animateTexture = 0;
    private Material material;
    public Texture2DArray TAM1;
    public Texture2DArray TAM2;
    public Texture2DArray TAM3;
    public Texture testTex;
    int propID;
    void Awake ()
    {
        material = new Material( Shader.Find("Hidden/LozowichEffectShader") );

    }

    private void Start()
    {

        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
        material.SetTexture("_TonalArtMap1", TAM1);
        material.SetTexture("_TonalArtMap2", TAM2);
        material.SetTexture("_TonalArtMap3", TAM3);
    }

    // Postprocess the image
    void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        //if (intensity == 0)
        //{
        //    Graphics.Blit (source, destination);
        //    return;
        //}



        material.SetInt("_animateTexture", animateTexture);
        material.SetFloat("_bwBlend", Mathf.Clamp(intensity,0f, 1f)); 
        material.SetFloat("_tile", Mathf.Clamp(tileAmount, 0f, 10f));
        material.SetFloat("_mitigationAmount", Mathf.Clamp(mitigationAmount, 0f, 100f));
        Graphics.Blit (source, destination, material);
    }


}