// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//

Shader "Custom/TAMShaderPlayer" {

    Properties {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Albedo", 2D) = "white" {}
        _TonalArtMap("Tonal art map", 2DArray) = "white" {}
        [NoScaleOffset] _NormalMap ("Normals", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Float) = 1
        [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.1
        _DetailTex ("Detail Texture", 2D) = "gray" {}
        [NoScaleOffset] _DetailNormalMap ("Detail Normals", 2D) = "bump" {}
        _DetailBumpScale ("Detail Bump Scale", Float) = 1
        _ContrastAdjustment("Contrast", Range(0,1)) = 1
        _Transparency("_Transparency", Range(0,1)) = 1.0
    }

    CGINCLUDE

    #define BINORMAL_PER_FRAGMENT

    ENDCG

    SubShader {

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
            ZTest LEqual
            Cull Off
                       
            CGPROGRAM

         
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

           
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
            #pragma multi_compile_fwdbase PLAYER
            
            
            // shadow helper functions and macros
            
            
            #pragma multi_compile DIRECTIONAL POINT
            #define PLAYER
            #include "TAMLightingShader.cginc"
           
            ENDCG
        }

        Pass {
            Tags {
                "LightMode" = "ForwardAdd" "Queue"="Transparent" "RenderType"="Transparent"
            }
            
            LOD 100
            Blend One SrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Off

            CGPROGRAM

            #pragma target 3.0

            
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile DIRECTIONAL POINT SPOT PLAYER
            
            #define PLAYER
            #include "TAMLightingShader.cginc"

            ENDCG
        }

        Pass {
            Tags {
                "LightMode" = "ShadowCaster"
            }

            CGPROGRAM

            #pragma target 3.0

            #pragma vertex MyShadowVertexProgram
            #pragma fragment MyShadowFragmentProgram

            #include "TAMShadowShader.cginc"

            ENDCG
        }
    }
}
//Shader "Custom/playerTAM"

//{
//    Properties {
//        _MainTex ("Main texture", 2D) = "white" {}
//        _TonalArtMap ("Tonal Art Map", 2DArray) = "" {}
//        _ColorTint ("Tint", Color) = (1.0, 0.0, 0.0, 1.0)
//        _Contrast("contrast", Range(1, 10)) = 1
//        _Levels("number of gradient levels", Range(0, 10))= 0
        
        
         
//    }
    
//    SubShader
//    {
    
//        Pass{
//            Tags {"Queue"="Transparent"}
//            ZWrite On
//            ColorMask 0
//        }
        
//        Pass
//        {
//            Tags {"LightMode"="ForwardBase" "Queue"="Transparent" "RenderType"="Transparent"}
            
//            LOD 100
//            Blend SrcAlpha OneMinusSrcAlpha

            
//            ZWrite Off
            
//            CGPROGRAM

         
//            #pragma vertex vert
//            #pragma fragment frag
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
            
//            #pragma multi_compile_fwdbase PLAYER
            
            
//            // shadow helper functions and macros
            
            
//            //#pragma multi_compile DIRECTIONAL POINT
//            #define PLAYER
//            #include "My_Lighting.cginc"
           
//            ENDCG
//        }
        
//        Pass {
//            Tags {
//                "LightMode" = "ForwardAdd" "Queue"="Transparent" "RenderType"="Transparent"
//            }
//            LOD 100

//            ZWrite Off
//            Blend SrcAlpha One
            
            
//            CGPROGRAM

//            #pragma target 3.0

//            #pragma vertex MyVertexProgram
//            #pragma fragment MyFragmentProgram
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
//            #include "AutoLight.cginc"
//            #pragma multi_compile_fwdadd_fullshadows
//            #pragma multi_compile DIRECTIONAL POINT SPOT PLAYER
//            // shadow helper functions and macros
           
//            #define PLAYER
            
//            #include "TAMLightingShader.cginc"
            
           

//            ENDCG
//        }
        
        
            
//        Pass{
        
//            Tags{"LightMode" = "ShadowCaster"}
            
//            CGPROGRAM
//            #pragma vertex MyShadowVertexProgram
//            #pragma fragment MyShadowFragmentProgram
//            #pragma multi_compile_shadowcaster
//            #include "UnityCG.cginc"
//            #include "AutoLight.cginc"
//            #pragma multi_compile _ SHADOWS_SCREEN
//            #pragma multi_compile _ VERTEXLIGHT_ON
//            #define PLAYER
//            #include "TAMShadowShader.cginc"
            
            
//            ENDCG
//        }
//    }
//}
