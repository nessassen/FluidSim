Shader "Unlit/FluidSim"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Width", Float) = 256
        _Height ("Height", Float) = 256
        _Damping ("Damping", Float) = .99
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

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Width;
            float _Height;
            float _Damping;
            
            float4 circle(float2 uv, float2 pos, float rad, float4 color) {
                float d = length(pos - uv) - rad;
                float t = clamp(d, 0.0, 1.0);
                return color * (1. - t);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //float2 size = float2(_Width, _Height);
                //float4 col = float4(0., 0., 0., 1.);
                //col -= circle(i.uv * size, size / 2., size.x / 4., float4(0., 0., 0., 1.));
                float4 bufCol = tex2D(_MainTex, i.uv);

                float2 north = tex2D(_MainTex, i.uv + float2(0., -2. / _Height)).br;
                north.x = north.x * (1 - north.y) + .5 * north.y;
                float2 south = tex2D(_MainTex, i.uv + float2(0., 2. / _Height)).br;
                south.x = south.x * (1 - south.y) + .5 * south.y;
                float2 west = tex2D(_MainTex, i.uv + float2(-2. / _Width, 0.)).br;
                west.x = west.x * (1 - west.y) + .5 * west.y;
                float2 east = tex2D(_MainTex, i.uv + float2(2. / _Width, 0.)).br;
                east.x = east.x * (1 - east.y) + .5 * east.y;

                float vel = (.5 - bufCol.g) * (1. - bufCol.r);
                float smooth = (north.x + south.x + east.x + west.x) / 4.;
                float b = (vel + smooth) * _Damping;
                float4 col = float4(0., bufCol.b, b, 1.);
                return col;
            }
            ENDCG
        }
    }
}