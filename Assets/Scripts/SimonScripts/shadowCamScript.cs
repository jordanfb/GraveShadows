using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowCamScript : MonoBehaviour
{
    // Start is called before the first frame update
    Camera shadowCam;


    void Awake()
    {


        shadowCam = GetComponent<Camera>();
        shadowCam.depthTextureMode = DepthTextureMode.Depth;


        Shader.SetGlobalTexture("_shadowDepthTexture", shadowCam.targetTexture);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
