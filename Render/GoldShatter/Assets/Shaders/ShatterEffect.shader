Shader "Custom/ShatterEffect" {
	Properties {
		_MainTint("Global Color Tint", Color) = (1,1,1,1)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		float4 _MainTint;

		struct Input {
			float2 uv_MainTex;
			float4 vertColor;
			
		};
		
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.vertColor = (1,1,1,1);
			v.vertex.xyz = v.vertex.xyz + v.normal.xyz * 2;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = (1,1,1); //IN.vertColor.rgb * _MainTint.rgb;
			o.Alpha = 1; //IN.vertColor.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
