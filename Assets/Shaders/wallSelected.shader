// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/wallSelected"

{
    Properties {
        _MainTex ("Main texture", 2D) = "white" {}
        _TonalArtMap ("Tonal Art Map", 2DArray) = "" {}
        _ColorTint ("Tint", Color) = (1.0, 0.0, 0.0, 1.0)
        _Contrast("contrast", Range(1, 10)) = 1
        _Levels("number of gradient levels", Range(0, 10))= 0
         
    }
    
    SubShader
    {
    
        
        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            CGPROGRAM

         
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
            #pragma multi_compile_fwdbase
            
            // shadow helper functions and macros
            
            
            //#pragma multi_compile DIRECTIONAL POINT
            #include "My_Lighting.cginc"
           
            ENDCG
        }
        
        Pass {
            Tags {
                "LightMode" = "ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile DIRECTIONAL POINT SPOT
            // shadow helper functions and macros
           
            
            //#pragma multi_compile_fwdadd 
            #include "My_Lighting.cginc"

           

            ENDCG
        }
        
        Pass{
            
            Blend SrcColor SrcAlpha
            
            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            
            struct v2f {

                float2 uv: TEXCOORD0;
                float4 pos : SV_POSITION;
                
                //float3 wNormal: TEXCOORD2;
            };
            
            v2f vert (appdata_base v)
            {
            
                v2f o;
                
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                
                return o;
            }
            
            fixed4 frag(v2f i): SV_Target{
                float4 color;
                half width =0.1;
                
                if(i.uv.y > fmod(_Time.y, 1.0)- width && i.uv.y < fmod(_Time.y, 1.0) + width){
                    half colorValue = abs(fmod(_Time.y, 1.0)-i.uv.y)- width;
                    half4 colorMultiplier = half4(colorValue,colorValue,colorValue,1.0);
                    
                    color = colorMultiplier;

                }else{
                    
                    color = float4(1.0,1.0,1.0,0.0);
                }
                
                return color;
             
            
            }

           

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
            
            #include "My_Shadows.cginc"
            
            
            ENDCG
        }
    }
}
