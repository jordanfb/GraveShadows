Shader "Custom/grabPassShadow"
{
    Properties
    {
        
        _ShadowTex ("shadowTex", 2D) = "white" {}
        _TonalArtMap ("Tonal Art Map", 2DArray) = "" {}
        _Transparency("Transparency", float) = 1.0
        
    }
    SubShader
    {
        // Draw ourselves after all opaque geometry
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha


        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }
        
        
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _BackgroundTexture;
            sampler2D _ShadowTex;
            float4 _ShadowTex_ST;
            float2 uv_ShadowTex;
            UNITY_DECLARE_TEX2DARRAY(_TonalArtMap);
            float4 _TonalArtMap_ST;
            float _Transparency;
            
            
            struct v2f
            {
                
                float4 grabPos : TEXCOORD1;
                float2 uv: TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _ShadowTex);
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            

            half4 frag(v2f i) : SV_Target
            {
                half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);
                
				if (tex2D(_ShadowTex, i.uv).r < 0.1) {
					return bgcolor;
				}
                else{
                    float4 finalColor = bgcolor * UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(i.uv, 5.0));
                    finalColor.a = _Transparency;
                    return finalColor;
                }
				
                //return tex2D(_ShadowTex, i.uv);
                
            }
		ENDCG
        }
		
    }
	//FallBack "Diffuse"
	//FallBack Off

}