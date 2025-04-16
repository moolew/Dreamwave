Shader "Custom/ScrollingTextureAlphaUI_CanvasGroup"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeedX ("Scroll Speed X", Float) = 0.1
        _ScrollSpeedY ("Scroll Speed Y", Float) = 0.0
        _ScaleX ("Scale X", Float) = 1.0
        _ScaleY ("Scale Y", Float) = 1.0
        _RealTime ("Real Time", Float) = 0.0
        _Alpha ("Alpha", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // 👈 This gives us CanvasGroup/RawImage color
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR; // Pass it to frag
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            float _ScaleX;
            float _ScaleY;
            float _RealTime;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float2 scaledUV = v.uv * float2(_ScaleX, _ScaleY);
                float2 scrolledUV = scaledUV + float2(_RealTime * _ScrollSpeedX, _RealTime * _ScrollSpeedY);
                o.uv = TRANSFORM_TEX(scrolledUV, _MainTex);

                o.color = v.color; // Pass vertex color (includes CanvasGroup alpha!)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= _Alpha;
                col *= i.color; // Multiply by vertex color (which includes CanvasGroup alpha)
                return col;
            }
            ENDCG
        }
    }
}
