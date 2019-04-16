Shader "Hidden/Custom/NoirEffectShader"
{
    Properties{
        _TAM1("TAM1", 2D) = "white" {}
    }
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_TAM1, sampler_TAM1);
        float _Blend;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
            color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);
            float2 TAMuv = float2(0,0);
            TAMuv.x = (4 * (1.0/8.0) + i.texcoord.x * (1.0/8.0));
            TAMuv.y = i.texcoord.y;
            color = SAMPLE_TEXTURE2D(_TAM1, sampler_TAM1, TAMuv);
            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}