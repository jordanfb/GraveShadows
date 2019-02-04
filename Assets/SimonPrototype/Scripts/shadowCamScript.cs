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
        print(shadowCam.targetTexture.name);
        shadowCam.depthTextureMode = DepthTextureMode.Depth;

        shadowCam.SetTargetBuffers(shadowCam.targetTexture.colorBuffer, shadowCam.targetTexture.depthBuffer);

        Shader.SetGlobalTexture("_shadowDepthTexture", shadowCam.targetTexture);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
