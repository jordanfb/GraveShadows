#if !defined(MY_LIGHTING_INCLUDED)
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
int _isSelected = 0.0;
float _Transparency = 1.0;

struct v2f {

    float2 uv: TEXCOORD0;
    float uv2: TEXCOORD1;
    SHADOW_COORDS(5)
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
    o.uv = TRANSFORM_TEX(v.texcoord, _TonalArtMap);
    o.uv2 = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    half3 wNormal = UnityObjectToWorldNormal(v.normal);
    TRANSFER_SHADOW(o);
 
    
    return o;
}

// normal map texture from shader properties

UnityLight CreateLight (v2f i) {
    UnityLight light;
    #if defined(POINT) || defined(SPOT)
        light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
        
        
    #else
        light.dir = _WorldSpaceLightPos0.xyz;
        
    #endif
    
    
    UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);
    
    
    light.color = _LightColor0.rgb * attenuation;
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
    
    
    UnityLight light = CreateLight(i);
    float3 dpdx = ddx(i.worldPos);
    float3 dpdy = ddy(i.worldPos);
    
    float3 flatNormal = -normalize(cross(dpdx, dpdy)).xyz;
    half nl = max(0, dot(flatNormal, light.dir));
    // factor in the light color
    //if(nl<0.2 || nl>1.0){
    //    nl=0;
    //}
    //else if(nl<0.8){
    //    nl=0.6;
    //}
    //else if(nl<0.95){
    //    nl=0.8;
    //}
    //else{
    //    nl=1.0;
    //}
    
    i.diff.rgb = nl * light.color;
    i.ambient = ShadeSH9(half4(flatNormal,1));
    i.ambient = 1.0;
    
    fixed shadow = SHADOW_ATTENUATION(i);
    fixed3 lighting = i.diff * shadow + i.ambient;
    
    //i.diff.a =1.0;
    fixed4 c;
    
    c.rgb = i.diff * lighting;
    c.a = 1.0;
    
    fixed l = Luminance(c);
    fixed texI = (1 - l) * _Levels;
    float2 worldUV;
    float2 worldUVMain;
    fixed4 col1;
    fixed4 col2;
    
    
    if(abs(flatNormal.y) > 0.5)
    {
        worldUV = TRANSFORM_TEX(i.worldPos.xz, _TonalArtMap);
        worldUVMain = TRANSFORM_TEX(i.worldPos.xz, _MainTex);
        col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
        col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

    }
    else if(abs(flatNormal.x) > 0.5)
    {
        worldUV = TRANSFORM_TEX(i.worldPos.yz, _TonalArtMap);
        worldUVMain = TRANSFORM_TEX(i.worldPos.yz, _MainTex);
        col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
        col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

    }
    else
    {
        worldUV = TRANSFORM_TEX(i.worldPos.xy, _TonalArtMap);
        worldUVMain = TRANSFORM_TEX(i.worldPos.xy, _MainTex);
        col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
        col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

    }
    
    col1 = AdjustContrast(col1, _Contrast) * _ColorTint;
    col2 = AdjustContrast(col2, _Contrast) * _ColorTint;
    
    
    float4 TAMcolor = lerp(col1, col2, texI - floor(texI))*tex2D(_MainTex, i.uv );
    //float4 TAMcolor = col1*tex2D(_MainTex, worldUVMain);
    
    //c = TAMcolor;
    
    #if defined(PLAYER)
        TAMcolor.a = _Transparency;
        return TAMcolor;
    #else
        TAMcolor.a =1.0;
        return TAMcolor;
    #endif
}

#endif

