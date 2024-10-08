Shader "Unlit/FluidSim"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Width", Float) = 512
        _Height ("Height", Float) = 512
        _Step ("Step", Float) = 2
        _Damping ("Damping", Float) = .99
    }
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent" 
        }
        ZTest Always
        ZWrite Off
        Blend One One
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
            float _Step;
            float _Damping;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 bufCol = tex2D(_MainTex, i.uv);

                float2 north = tex2D(_MainTex, frac(i.uv + float2(0., -_Step))).br;
                north.x = north.x * (1. - north.y) + .5 * north.y;
                float2 south = tex2D(_MainTex, frac(i.uv + float2(0., _Step))).br;
                south.x = south.x * (1. - south.y) + .5 * south.y;
                float2 west = tex2D(_MainTex, frac(i.uv + float2(-_Step, 0.))).br;
                west.x = west.x * (1. - west.y) + .5 * west.y;
                float2 east = tex2D(_MainTex, frac(i.uv + float2(_Step, 0.))).br;
                east.x = east.x * (1. - east.y) + .5 * east.y;

                float smooth = (north.x + south.x + east.x + west.x) - 2.;
                float vel = (.5 - bufCol.g) * (1. - bufCol.r) * 2.;

                float g = bufCol.b * (1. - bufCol.r) + (bufCol.b * 2. - bufCol.r / 2.) * bufCol.r;
                float b = (smooth + vel) * _Damping * .5 + .5;
                float4 col = float4(0., g, b, 1.);
                return col;
            }
            ENDCG
        }
    }
}