Shader "Custom/TonalArtMapShader" {
	Properties {
		_MainTex ("Main texture", 2D) = "white" {}
		_TonalArtMap ("Tonal Art Map", 2DArray) = "" {}
		_ColorTint ("Tint", Color) = (1.0, 0.6, 0.6, 1.0)
		_Test ("Test", Range(0, 10)) = 0
        _Mod("modifier", Range(1, 10)) = 1
        _Levels("number of gradient levels", Range(0, 10))= 0
        _RampTex("Ramp", 2D) = ""{}
	}

	SubShader {
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM

		#pragma surface surf Lambert finalcolor:apply_tam
        
		struct Input {
            float3 worldPos;
			float2 uv_MainTex;
			float2 uv_TonalArtMap;
            float3 worldNormal;
            
		};

		float _Test;
		fixed4 _ColorTint;
		sampler2D _MainTex;
        float _Mod;
        float _Levels;
        float2 worldUV;
        
        sampler2D _RampTex;
      
        fixed4 LightingToon(SurfaceOutput s, fixed3 lightDir, fixed atten){
            half NdotL = dot(s.Normal, lightDir);
            
            NdotL = tex2D(_RampTex, fixed2(NdotL, 0.5));
            
            half4 color;
            
            color.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
            color.a = s.Alpha;
            
            return color;
        
        }
      
      
      
		//float4 _MainTex_ST;
		UNITY_DECLARE_TEX2DARRAY(_TonalArtMap);
        float4 _TonalArtMap_ST;
		// yeah, yeah, i know, something something gamma. i don't care
		// at the moment.
		float luma(fixed4 color) {
			const fixed4 luma_vec = float4(0.2126, 0.7152, 0.0722, 1.0);
			return dot(color, luma_vec);
		}

		void do_nothing(Input IN, SurfaceOutput o, inout fixed4 color)
		{
		}
        
        
        
		void apply_tam(Input IN, SurfaceOutput o, inout fixed4 color)
		{
			//fixed l = pow(luma(color), 2.2);
			fixed l = luma(color);
			fixed texI = (1 - l) * _Levels;
            float2 worldUV = IN.uv_TonalArtMap;
            if(abs(IN.worldNormal.y) > 0.5)
            {
                float2 worldUV = TRANSFORM_TEX(IN.worldPos.xz, _TonalArtMap);
                fixed4 col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
                fixed4 col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

                color = pow(lerp(col1, col2, texI - floor(texI)),_Mod);
            }
            else if(abs(IN.worldNormal.x) > 0.5)
            {
                float2 worldUV = TRANSFORM_TEX(IN.worldPos.yz, _TonalArtMap);
                fixed4 col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
                fixed4 col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

                color = pow(lerp(col1, col2, texI - floor(texI)),_Mod);
            }
            else
            {
                float2 worldUV = TRANSFORM_TEX(IN.worldPos.xy, _TonalArtMap);
                fixed4 col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, floor(texI)));
                fixed4 col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(worldUV, ceil(texI)));

                color = pow(lerp(col1, col2, texI - floor(texI)),_Mod);
            }
            //fixed4 col1 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(IN.uv_TonalArtMap, floor(texI)));
            //fixed4 col2 = UNITY_SAMPLE_TEX2DARRAY(_TonalArtMap, float3(IN.uv_TonalArtMap, ceil(texI)));

            //color = pow(lerp(col1, col2, texI - floor(texI)),_Mod);
            
			
			
		}

		void surf (Input IN, inout SurfaceOutput o) {
           
			o.Albedo = _ColorTint * tex2D(_MainTex, IN.uv_MainTex);
		}
        

		ENDCG
	}
	Fallback "Diffuse"
}
