Shader "Custom/shadowShader"
{
    Properties
    {
        _CameraTex ("Camera texture", 2D) = "" {}

    }
    SubShader
    {
        Pass{
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma vertex vert
        #pragma fragment frag
        
        sampler2D _CameraTex;
        float4 _Color;
        uniform sampler2D _shadowDepthTexture;
        uniform sampler2D _CameraDepthTexture;
        
        struct vertexInput{
            float4 pos: POSITION;
            float2 texcoord: TEXCOORD0;
            float3 normal : NORMAL;
        };
        
        struct vertexOutput{
            float4 pos: SV_POSITION;
            float2 texcoord: TEXCOORD0;
            float3 normal : NORMAL;
            float3 eyePos : float3;
        };
        
        vertexOutput vert(vertexInput input){
            vertexOutput o;
            o.pos = UnityObjectToClipPos(input.pos);
            o.texcoord = input.texcoord;
            o.eyePos = mul(UNITY_MATRIX_MV, o.pos);
            return o;
        }
        
       
        
        float4 frag(vertexOutput o): COLOR{
        
            
            float4 c = tex2D(_CameraTex, o.texcoord);
            
            if(c.r>0.1){
                return float4(c.r, c.r, c.r, 1.0);
            }else{
                return float4(1.0, 1.0,1.0,1.0);
            }
              
               
        }
        
        
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        
        ENDCG
    }
    }
    FallBack "Diffuse"
}
