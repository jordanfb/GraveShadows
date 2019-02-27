// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/playerFlat"

{
    Properties {
        _MainTex ("Main texture", 2D) = "white" {}
        _TonalArtMap ("Tonal Art Map", 2DArray) = "" {}
        _ColorTint ("Tint", Color) = (1.0, 0.0, 0.0, 1.0)
        _Contrast("contrast", Range(1, 10)) = 1
        _Levels("number of gradient levels", Range(0, 10))= 0
        _Transparency("_Transparency", Range(0,1)) =1.0
         
    }
    
    SubShader
    {
    
        Pass{
            Tags {"Queue"="Transparent"}
            ZWrite On
            ColorMask 0
        }
        
        Pass
        {
            Tags {"LightMode"="ForwardBase" "Queue"="Transparent" "RenderType"="Transparent"}
            
            LOD 100
            Blend SrcAlpha OneMinusSrcAlpha

            
            ZWrite Off
            
            CGPROGRAM

         
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
            #pragma multi_compile_fwdbase PLAYER
            
            
            // shadow helper functions and macros
            
            
            //#pragma multi_compile DIRECTIONAL POINT
            #define PLAYER
            #include "My_Lighting.cginc"
           
            ENDCG
        }
        
        Pass {
            Tags {
                "LightMode" = "ForwardAdd" "Queue"="Transparent" "RenderType"="Transparent"
            }
            LOD 100

            ZWrite Off
            Blend SrcAlpha One
            
            
            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile DIRECTIONAL POINT SPOT PLAYER
            // shadow helper functions and macros
           
            #define PLAYER
            
            #include "My_Lighting.cginc"
            
           

            ENDCG
        }
        
        
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
            
        Pass{
        
            Tags{"LightMode" = "ShadowCaster"}
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile _ SHADOWS_SCREEN
            #pragma multi_compile _ VERTEXLIGHT_ON
            #define PLAYER
            #include "My_Shadows.cginc"
            
            
            ENDCG
        }
    }
}
