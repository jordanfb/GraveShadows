Shader "Custom/shadowShader"
{
    Properties
    {
        _CameraTex ("Camera texture", 2D) = "" {}

    }
    SubShader
    {
        Pass{
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
        
        sampler2D _CameraTex;
        float4 _Color;
        uniform sampler2D _shadowDepthTexture;
        uniform sampler2D _CameraDepthTexture;
        
        struct vertexInput{
            float4 pos: POSITION;
            float2 texcoord: TEXCOORD0;
            float3 normal : NORMAL;
            float3 viewT : TANGENT;
        };
        
        struct vertexOutput{
            float4 pos: SV_POSITION;
            float2 texcoord: TEXCOORD0;
            float3 normal : NORMAL;
            float3 eyePos : float3;
            float3 viewT : TANGENT;
        };
        
        vertexOutput vert(vertexInput input){
            vertexOutput o;
            o.pos = UnityObjectToClipPos(input.pos);
            o.texcoord = input.texcoord;
            o.normal = normalize(input.normal);
            o.viewT = ObjSpaceViewDir(o.pos);
            return o;
        }
        
       
        
        float4 frag(vertexOutput o): SV_Target{
        
            
            float4 c = tex2D(_CameraTex, o.texcoord);
            
            if(c.r<0.1){
            
                c.a = 0.0;
                
            }
            
            return c;

               
        }
        

        
        ENDCG
        }
    }
    FallBack "Diffuse"
}
