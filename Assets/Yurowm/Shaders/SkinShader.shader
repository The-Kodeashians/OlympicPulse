Shader "Yurowm/SkinShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        Cull Back
        CGPROGRAM
        #pragma surface surf Lambert

        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
        };
        sampler2D _MainTex;
        float4 _Color;

        void surf (Input IN, inout SurfaceOutput o) {
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            float3 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * _Color.rgb; // Applying the color to the albedo
            o.Emission = lerp(0.2, 1.0, pow (rim, 2)) * c.rgb * _Color.rgb; // Applying the color to the emission
        }
        ENDCG
    }
    Fallback "Diffuse"
}
