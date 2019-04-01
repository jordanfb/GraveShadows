// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED

#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"

float4 _Tint;
sampler2D _MainTex;
UNITY_DECLARE_TEX2DARRAY(_TonalArtMap);
float4 _MainTex_ST, _TonalArtMap_ST;


sampler2D _NormalMap, _DetailNormalMap;
float _BumpScale, _DetailBumpScale;

float _Metallic;
float _Smoothness;

struct VertexData {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float2 uv : TEXCOORD0;
};

struct Interpolators {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : TEXCOORD1;
    float3 worldNormal: TEXCOORD7;
	#if defined(BINORMAL_PER_FRAGMENT)
		float4 tangent : TEXCOORD2;
	#else
		float3 tangent : TEXCOORD2;
		float3 binormal : TEXCOORD3;
	#endif

	float3 worldPos : TEXCOORD4;

	SHADOW_COORDS(5)

	#if defined(VERTEXLIGHT_ON)
		float3 vertexLightColor : TEXCOORD6;
	#endif
};

void ComputeVertexLightColor (inout Interpolators i) {
	#if defined(VERTEXLIGHT_ON)
		i.vertexLightColor = Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb,
			unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, i.worldPos, i.normal
		);
	#endif
}

float3 CreateBinormal (float3 normal, float3 tangent, float binormalSign) {
	return cross(normal, tangent.xyz) *
		(binormalSign * unity_WorldTransformParams.w);
}

Interpolators MyVertexProgram (VertexData v) {
	Interpolators i;
	i.pos = UnityObjectToClipPos(v.vertex);
	i.worldPos = mul(unity_ObjectToWorld, v.vertex);
	i.normal = UnityObjectToWorldNormal(v.normal);

	#if defined(BINORMAL_PER_FRAGMENT)
		i.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
	#else
		i.tangent = UnityObjectToWorldDir(v.tangent.xyz);
		i.binormal = CreateBinormal(i.normal, i.tangent, v.tangent.w);
	#endif

	i.uv = TRANSFORM_TEX(v.uv, _MainTex);
    
	TRANSFER_SHADOW(i);
    i.worldNormal = UnityObjectToWorldNormal(v.normal);
	ComputeVertexLightColor(i);
	return i;
}

UnityLight CreateLight (Interpolators i) {
	UnityLight light;

	#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
		light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
	#else
		light.dir = _WorldSpaceLightPos0.xyz;
	#endif
    float3 lightVec = _WorldSpaceLightPos0.xyz - i.worldPos;
	UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);
    //fixed attenuation = 1/dot(lightVec, lightVec);
    
	light.color = _LightColor0.rgb * attenuation;
	light.ndotl = DotClamped(i.normal, light.dir);
	return light;
}

UnityIndirect CreateIndirectLight (Interpolators i) {
	UnityIndirect indirectLight;
	indirectLight.diffuse = 0;
	indirectLight.specular = 0;

	#if defined(VERTEXLIGHT_ON)
		indirectLight.diffuse = i.vertexLightColor;
	#endif

	#if defined(FORWARD_BASE_PASS)
		indirectLight.diffuse += max(0, ShadeSH9(float4(i.normal, 1)));
	#endif

	return indirectLight;
}


float luma(fixed4 color) {
    const fixed4 luma_vec = float4(0.2126, 0.7152, 0.0722, 1.0);
    return dot(color, luma_vec);
}

float _ContrastAdjustment;
float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
    
    UnityLight light = CreateLight(i);
   
    
	float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
    fixed4 col1;
    fixed4 col2;
   //float3 worldUV;
    
    
    
    
    float3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Tint.rgb ;
    float3 specularTint = float3(0.0, 1.0,0.0);
    float oneMinusReflectivity = 0.0;
    //albedo = DiffuseAndSpecularFromMetallic(
    //    albedo, _Metallic, specularTint, oneMinusReflectivity
    //);

    half4 c = UNITY_BRDF_PBS(
        albedo, specularTint,
        oneMinusReflectivity, _Smoothness,
        i.normal, viewDir,
        CreateLight(i), CreateIndirectLight(i)
    );
    
    
    fixed l = Luminance(c);
    l = pow(l, _ContrastAdjustment);
    fixed texI = (1 - l) * 8.0;
    float2 worldUV;
    float levels = 8.0;
    
    if(abs(i.worldNormal.y) > 0.5)
    {
        worldUV = TRANSFORM_TEX(i.worldPos.xz, _TonalArtMap);
        col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
        col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

    }
    else if(abs(i.worldNormal.x) > 0.5)
    {
        worldUV = TRANSFORM_TEX(i.worldPos.yz, _TonalArtMap);
        col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
        col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

    }
    else
    {
        worldUV = TRANSFORM_TEX(i.worldPos.xy, _TonalArtMap);
        col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
        col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

    }
    
	float4 TAMcolor = lerp(col1, col2, texI - floor(texI));
    
    return TAMcolor;
    
    
	
}

#endif