Shader "UI/ImageGaussianBlur"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BlurSize ("Blur Size", Range(0, 10)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            float _BlurSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
{
    float2 texel = _MainTex_TexelSize.xy * _BlurSize;
    fixed4 color = 0;

    // 5x5 Gaussian kernel
    color += tex2D(_MainTex, i.uv + texel * float2(-2, -2)) * 1.0;
    color += tex2D(_MainTex, i.uv + texel * float2(-1, -2)) * 4.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 0, -2)) * 6.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 1, -2)) * 4.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 2, -2)) * 1.0;

    color += tex2D(_MainTex, i.uv + texel * float2(-2, -1)) * 4.0;
    color += tex2D(_MainTex, i.uv + texel * float2(-1, -1)) * 16.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 0, -1)) * 24.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 1, -1)) * 16.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 2, -1)) * 4.0;

    color += tex2D(_MainTex, i.uv + texel * float2(-2,  0)) * 6.0;
    color += tex2D(_MainTex, i.uv + texel * float2(-1,  0)) * 24.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 0,  0)) * 36.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 1,  0)) * 24.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 2,  0)) * 6.0;

    color += tex2D(_MainTex, i.uv + texel * float2(-2,  1)) * 4.0;
    color += tex2D(_MainTex, i.uv + texel * float2(-1,  1)) * 16.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 0,  1)) * 24.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 1,  1)) * 16.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 2,  1)) * 4.0;

    color += tex2D(_MainTex, i.uv + texel * float2(-2,  2)) * 1.0;
    color += tex2D(_MainTex, i.uv + texel * float2(-1,  2)) * 4.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 0,  2)) * 6.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 1,  2)) * 4.0;
    color += tex2D(_MainTex, i.uv + texel * float2( 2,  2)) * 1.0;

    // 所有權重總和為 256
    return color / 256.0 * i.color;
}
            ENDCG
        }
    }
}