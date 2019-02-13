Shader "Custom/sufShadowShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _TonalArtMap ("Tonal Art Map", 2DArray) = "" {}
        
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        UNITY_DECLARE_TEX2DARRAY(_TonalArtMap);
        float4 _TonalArtMap_ST;

        struct Input
        {
            float2 uv_MainTex;
        };

        
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            if(c.r<0.2){
               c.a = 0.0;
            }
            else{
                c = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(IN.uv_MainTex, 4.0));
            }
            o.Albedo = c;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
