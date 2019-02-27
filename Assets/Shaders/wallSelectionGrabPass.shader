Shader "Custom/wallSelectionGrabPass"
{
    Properties
    {
        
        
    }
    SubShader
    {
        // Draw ourselves after all opaque geometry
        Tags { "Queue" = "Transparent" }

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }
        
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata_base v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            sampler2D _BackgroundTexture;

            half4 frag(v2f i) : SV_Target
            {
                
                half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);
                float width = 0.1;
                half4 color;
                if(i.uv.y > fmod(_Time.y, 1.0)- width && i.uv.y < fmod(_Time.y, 1.0) + width){
                    float colorValue = abs(fmod(_Time.y, 1.0)-i.uv.y)- width;
                    half4 colorMultiplier = half4(colorValue,colorValue,colorValue,1.0);
                    
                    color = bgcolor + colorMultiplier*2.0;
                    //color = 1-bgcolor;
                    
                  

                }else{
                    
                    color = bgcolor;
                }
                return color;
            }
            ENDCG
        }
        
    }
    //FallBack "Diffuse"
    //FallBack Off

}