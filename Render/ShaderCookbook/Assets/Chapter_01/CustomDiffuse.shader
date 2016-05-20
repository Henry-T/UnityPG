Shader "Cookbook/CustomDiffuse" {
	Properties {
		_EmissiveColor ("Emissive Color", Color) = (1,1,1,1)
		_AmbientColor ("Ambient Color", Range(0,10)) = 2
		_MySliderValue ("This is a Slider", Range(0,10)) = 2.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// surface 后面是个列表 我还以为surf是关键词，函数删了一直报错..
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
			// dot两向量越靠拢投影值越大
			float difLight = max(0, dot (s.Normal, lightDir));
			float4 col;
			// _LightColor0 是Unity提供的主光源
			col.rgb = s.Albedo * _LightColor0.rgb * (difLight * atten * 2);
			col.a = s.Alpha;  return col;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
