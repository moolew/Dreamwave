Shader "Unlit/TiledMirrorFixed"
{
    Properties
    {
        _MainTex ("Render Texture", 2D) = "white" {}
        _Zoom ("Zoom Level", Float) = 1.0
        _OffsetX ("Scroll X", Float) = 0.0
        _OffsetY ("Scroll Y", Float) = 0.0
        _Rotation ("Rotation (Z degrees)", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float _Zoom;
            float _OffsetX;
            float _OffsetY;
            float _Rotation;

            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 rotateFromCenter(float2 uv, float angleDeg, float aspect)
            {
                // Convert from UV (0–1) to NDC-like (-1 to 1)
                float2 pt = (uv - 0.5) * 2.0;
                pt.x *= aspect; // Apply aspect ratio

                // Rotate in screen space
                float rad = radians(angleDeg);
                float s = sin(rad);
                float c = cos(rad);
                float2 rotated = float2(
                    pt.x * c - pt.y * s,
                    pt.x * s + pt.y * c
                );

                rotated.x /= aspect; // Remove aspect distortion
                // Convert back to UV (0–1)
                return (rotated * 0.5) + 0.5;
            }

            float4 frag (v2f i) : SV_Target
            {
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 uv = rotateFromCenter(i.uv, _Rotation, aspect);

                // Center and zoom
                float2 zoomedUV = (uv - 0.5) * _Zoom;
                zoomedUV.x += _OffsetX;
                zoomedUV.y += _OffsetY;

                // Tiling
                float2 tileIndex = floor(zoomedUV + 0.5);
                float2 localUV = frac(zoomedUV + 0.5);

                if ((int)abs(tileIndex.x) % 2 == 1)
                    localUV.x = 1.0 - localUV.x;
                if ((int)abs(tileIndex.y) % 2 == 1)
                    localUV.y = 1.0 - localUV.y;

                return tex2D(_MainTex, localUV);
            }
            ENDCG
        }
    }
}
