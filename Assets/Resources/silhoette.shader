// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Outlined/Silhouetted Diffuse" {
    Properties {
        _Color ("Main Color", Color) = (.5,.5,.5,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Outline ("Outline width", Range (0.0, 1)) = .005
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _PerlinNoise("PerlinTex", 2D) ="white" {}
    }
 
CGINCLUDE
#include "UnityCG.cginc"
 
struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 texcoord : TEXCOORD0;
};
 
struct v2f {
    float4 pos : POSITION;
    float3 normal : NORMAL;
    float4 color : COLOR;
};
 
uniform float _Outline;
uniform float4 _OutlineColor;
uniform sampler2D _PerlinNoise;

v2f vert(appdata_full v) {
    // just make a copy of incoming vertex data but scaled according to normal direction
    v2f o;
    float4 dist = distance(_WorldSpaceCameraPos, mul(UNITY_MATRIX_MV, v.vertex).xyz);
    float4 pUV = v.texcoord;
    pUV.x = pUV.x * sin(_Time.y);
    pUV.y = pUV.y * cos(_Time.y);
    float4 perlinSample = tex2Dlod(_PerlinNoise, pUV);
    
    
    //v.vertex.xyz += (v.normal * (perlinSample.x - 0.5))*0.1;
    o.pos = UnityObjectToClipPos(v.vertex);
 
    float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
    float2 offset = TransformViewToProjection(norm.xy);
    
    
   
    
    
    o.pos.xy += offset * (o.pos.z + sin(_Time.z)*0.6+0.6) * (_Outline);
    
    o.color = _OutlineColor;
    return o;
}


ENDCG
 
    SubShader {
        Tags { "Queue" = "Transparent" }
 
        // note that a vertex shader is specified here but its using the one above
        Pass {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Off
            ZWrite Off
            ZTest Always
            ColorMask RGB // alpha not used
 
            // you can choose what kind of blending mode you want for the outline
            Blend SrcAlpha OneMinusSrcAlpha // Normal
            //Blend One One // Additive
            //Blend One OneMinusDstColor // Soft Additive
            //Blend DstColor Zero // Multiplicative
            //Blend DstColor SrcColor // 2x Multiplicative
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            half4 frag(v2f i) :COLOR {
                
                
                
                
                return i.color;
            }
            ENDCG
        }
 
        Pass {
            Name "BASE"
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            Material {
                Diffuse [_Color]
                Ambient [_Color]
            }
            Lighting On
            SetTexture [_MainTex] {
                ConstantColor [_Color]
                Combine texture * constant
            }
            SetTexture [_MainTex] {
                Combine previous * primary DOUBLE
            }
        }
    }
 
    
 
    Fallback "Diffuse"
}