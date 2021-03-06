﻿#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED

//#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"



sampler2D _MainTex;
UNITY_DECLARE_TEX2DARRAY(_TonalArtMap);
float4 _TonalArtMap_ST;
float4 _MainTex_ST;
float _Levels;
float _Contrast;
float4 _ColorTint;
float2 uv_TonalArtMap;
float _Transparency;
float _AttenMod;
float _Ambient;
struct v2f {

    float2 uv: TEXCOORD0;
    float uv2: TEXCOORD1;
    float3 worldPos : TEXCOORD3;
    // these three vectors will hold a 3x3 rotation matrix
    // that transforms from tangent to world space
    
    float4 pos : SV_POSITION;
    float3 diff: COLOR0;
    float3 ambient: COLOR1;
    float3 normal: NORMAL;
    
    //float3 wNormal: TEXCOORD2;
};



// vertex shader now also needs a per-vertex tangent vector.
// in Unity tangents are 4D vectors, with the .w component used to
// indicate direction of the bitangent vector.
// we also need the texture coordinate.
v2f vert (appdata_base v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.uv2 = TRANSFORM_TEX(v.texcoord, _TonalArtMap);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    half3 wNormal = UnityObjectToWorldNormal(v.normal);
 
    
    return o;
}


UnityLight CreateLight (v2f i) {
    UnityLight light;
    
    light.dir = _WorldSpaceLightPos0.xyz;
    
        
    //UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);
    
    light.color = _LightColor0.rgb;
    
    //light.ndotl = DotClamped(i.normal, light.dir);
    
     return light;
}





float luma(fixed4 color) {
    const fixed4 luma_vec = float4(0.2126, 0.7152, 0.0722, 1.0);
    return dot(color, luma_vec);
}

half4 AdjustContrast(half4 color, half contrast) {
    return saturate(lerp(half4(0.5, 0.5, 0.5, 1.0), color, contrast));
}

fixed4 frag (v2f i) : SV_Target
{
    
    
    //UnityLight light = CreateLight(i);
    //float3 dpdx = ddx(i.worldPos);
    //float3 dpdy = ddy(i.worldPos);
    
    //float3 flatNormal = -normalize(cross(dpdx, dpdy)).xyz;
    //half nl = max(0, dot(flatNormal, light.dir));
    
    
    //i.diff.rgb = light.color * nl ;
    ////i.ambient = ShadeSH9(half4(flatNormal,1));
    
    
    ////fixed shadow = SHADOW_ATTENUATION(i);
    //fixed3 lighting = i.diff;
    
    ////lighting = lighting + i.ambient;
    
    ////i.diff.a =1.0;
    //fixed4 c;
    
    //c.rgb = i.diff * lighting * tex2D(_MainTex, i.uv);
    
    ////c.a = 1.0;
    
    //fixed l = Luminance(c);
    //fixed texI = (1 - l) * _Levels;
    //float2 worldUV;
    //float2 worldUVMain;
    //fixed4 col1;
    //fixed4 col2;
    
    
    //if(abs(flatNormal.y) > 0.5)
    //{
    //    worldUV = TRANSFORM_TEX(i.worldPos.xz, _TonalArtMap);
    //    col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
    //    col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

    //}
    //else if(abs(flatNormal.x) > 0.5)
    //{
    //    worldUV = TRANSFORM_TEX(i.worldPos.yz, _TonalArtMap);
    //    col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
    //    col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

    //}
    //else
    //{
    //    worldUV = TRANSFORM_TEX(i.worldPos.xy, _TonalArtMap);
    //    col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
    //    col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

    //}
    
    //col1 = AdjustContrast(col1, _Contrast) * _ColorTint;
    //col2 = AdjustContrast(col2, _Contrast) * _ColorTint;
    
    
    //float4 TAMcolor = lerp(col1, col2, texI - floor(texI));
    ////float4 TAMcolor = col1*tex2D(_MainTex, worldUVMain);
    

    //c.rgb += _Ambient;

    //#if defined(PLAYER)
    //    c.a = _Transparency;
    //    return c;
    //#else
    //    c.a = 1.0;
        
    //    return c;
    //#endif
    
    return _Ambient;
}

#endif

