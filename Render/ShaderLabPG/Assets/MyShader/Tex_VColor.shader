Shader "Custom/Tex_VColor" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _VertColor ("Vertex Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader {
        BindChannels {
            Bind "Vertex", vertex
            Bind "texcoord", texcoord
            Bind "Color", color
        } 
        Pass {
            Tags {"LightMode" = "Vertex"}
            Color [_PPLAmbient]
            SetTexture [_MainTex] {constantColor [_Color] combine texture * primary, texture * constant }
        }
    }
	FallBack "Diffuse"
}
