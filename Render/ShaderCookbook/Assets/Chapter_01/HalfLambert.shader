﻿Shader "Cookbook/HalfLambert" {
	Properties {
		_EmissiveColor ("Emissive Color", Color) = (1,1,1,1)
		_AmbientColor ("Ambient Color", Range(0,10)) = 2
		_MySliderValue ("This is a Slider", Range(0,10)) = 2.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BasicDiffuse

		float4 _EmissiveColor;
		float4 _AmbientColor;
		float  _MySliderValue;
			
		struct Input {
			float4 _EmissiveColor;
			float4 _AmbientColor;
			float  _MySliderValue;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float4 c;
			c = pow((_EmissiveColor + _AmbientColor), _MySliderValue);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		inline float4 LightingBasicDiffuse (SurfaceOutput s, fixed3 lightDir, fixed atten) 
		{
			float difLight = max(0, dot (s.Normal, lightDir));
			float hLambert = difLight * 0.5 + 0.5;

			float4 col;
			col.rgb = s.Albedo * _LightColor0.rgb * (hLambert * atten * 2);
			col.a = s.Alpha;  return col;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
