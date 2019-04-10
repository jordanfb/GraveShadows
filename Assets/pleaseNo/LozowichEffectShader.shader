Shader "Hidden/LozowichEffectShader" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _bwBlend ("Black & White blend", Range (0, 1)) = 0
        _tile ("tileAmount", Range (0, 10)) = 1
        _TonalArtMap("Tonal art map", 2DArray) = "white" {}
        
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float _bwBlend;
            uniform float _tile;
            UNITY_DECLARE_TEX2DARRAY(_TonalArtMap);
            
            float4 frag(v2f_img i) : COLOR {
                float4 c = tex2D(_MainTex, i.uv);
                
                float lum = c.r*.3 + c.g*.59 + c.b*.11;
                lum*=1.8;
                float3 bw = float3( lum, lum, lum ); 
                
                float4 result = c;
                
                fixed texI = (1 - lum) * 8.0;
                float4 col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(i.uv * _tile, floor(texI)));
                float4 col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(i.uv * _tile, ceil(texI)));
                float4 TAMcolor = lerp(col1, col2, texI - floor(texI));
                result.rgb = lerp(c.rgb, TAMcolor, _bwBlend);
                return result;
            }
            ENDCG
        }
    }
}