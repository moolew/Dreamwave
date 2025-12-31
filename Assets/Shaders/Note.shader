Shader "Dreamwave/NoteScrollColored"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _RotateZ ("Rotate Z (Degrees)", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Tint;

            float _SongTimeMs;
            float _ScrollSpeed;
            float _StrumLineY;
            float _NoteTimeMs;
            float _SpawnY;
            float _RotateZ;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;

                float y = _StrumLineY - (_NoteTimeMs - _SongTimeMs) * _ScrollSpeed;

                float rad = radians(_RotateZ);
                float s = sin(rad);
                float c = cos(rad);

                float2 p = v.vertex.xy;

                // rotate in local 2D space
                float2 r;
                r.x = p.x * c - p.y * s;
                r.y = p.x * s + p.y * c;

                float3 pos = float3(r.x, r.y + y + _SpawnY, v.vertex.z);

                o.pos = UnityObjectToClipPos(float4(pos,1));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Tint;
                return col;
            }
            ENDCG
        }
    }
}
