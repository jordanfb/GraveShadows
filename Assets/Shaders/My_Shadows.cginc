// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#if !defined(MY_SHADOWS_INCLUDED)
#define MY_SHADOWS_INCLUDED

//#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"
#include "UnityCG.cginc"
#include "Lighting.cginc"
struct VertexData{
    float4 position: POSITION;
    float3 normal: NORMAL;

    
};





float4 vert (VertexData v) : SV_POSITION
{
    float4 position = UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
    //float4 position = 0;
    //return float4(1.0,0.0,0.0);
    return UnityApplyLinearShadowBias(position);

    
    
}




fixed4 frag () : SV_Target
{

    return 0;
}

#endif

