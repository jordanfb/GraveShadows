Shader "Custom/wallSelectionGrabPass"
{
    Properties
    {
        
        _PerlinTex("Perlin Noise Texture", 2D) = "white" {}
        _FlowMap("Flow Map", 2D) = "white" {}
    }
    SubShader
    {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }
        
        
        Pass
        {
            
            //Blend DstColor Zero
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            
            
            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD1;
                
            };
            
            sampler2D _PerlinTex, _FlowMap;
            float4 _PerlinTex_ST, _FlowMap_ST;
            
            v2f vert(appdata_base v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_PerlinTex);
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _BackgroundTexture;

            half4 frag(v2f i) : SV_Target
            {
                float threshold = 0.4*sin(_Time.w)+0.5;
                float4 flowSample = tex2D(_FlowMap, i.uv);
                float4 perlinSample = tex2D(_PerlinTex, i.uv);
                
                
                fixed4 c = tex2Dproj(_BackgroundTexture, UNITY_PROJ_COORD(i.grabPos));
                c-=0.4;
                fixed val = 1 - tex2D(_PerlinTex, i.uv).r;
                if(val < threshold - 0.04)
                {
                    discard;
                }
 
                bool b = val < threshold;
                return lerp(c, c * fixed4(lerp(1, 0, 1 - saturate(abs(threshold - val) / 0.04)), 0, 0, 1), b);
                return c;
            }
            ENDCG
        }
        
    }
    //FallBack "Diffuse"
    //FallBack Off

}