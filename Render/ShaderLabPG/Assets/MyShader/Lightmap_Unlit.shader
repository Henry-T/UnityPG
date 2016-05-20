Shader "NewShader/Lightmap/Unlit"{
  Properties{
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Lightmap ("Lightmap", 2D) = "black" {}
  }

  SubShader{
    Tags {"RenderType"="Opaque"}
    LOD 100

    Pass{
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      struct appdata{
        float4 vertex : POSITION;
        half2 uv : TEXCOORD0;
        half2 uv1: TEXCOORD1;
      };

      struct v2f {
        float4 pos : SV_POSITION;
        half2 uv :TEXCOORD0;
        half2 uv1:TEXCOORD1;
      };

      sampler2D _MainTex;
      sampler2D _Lightmap;

      v2f vert(appdata v)
      {
        v2f o;
        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
        o.uv = v.uv;
        o.uv1 = v.uv1;
        return o;
      }

      half4 frag(v2f i) : COLOR
      {
        half4 c = tex2D(_MainTex, i.uv);
        half4 c1= tex2D(_Lightmap, i.uv1);
        half4 o;
        o.rgb = c.rgb * c1.rgb;
        o.a = 1;
        return o;
      }
      ENDCG
    }
  }
}