Shader "Unlit/FluidEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FluidTex("Fluid", 2D) = "black" {}
        _Width ("Width", Float) = 256
        _Height ("Height", Float) = 256
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
            sampler2D _FluidTex;
            float _Width;
            float _Height;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //float Xoffset = tex2D(_FluidTex, i.uv + float2(-1/_Height, 0)).b - tex2D(_FluidTex, i.uv + float2(1/_Height, 0)).b;
                //float Yoffset = tex2D(_FluidTex, i.uv + float2(0, -1/_Height)).b - tex2D(_FluidTex, i.uv + float2(0, 1/_Height)).b;
                //fixed4 col = tex2D(_MainTex, i.uv + float2(Xoffset/_Height, Yoffset/_Height) * 32); //Doesn't work yet
                fixed2 fc = tex2D(_FluidTex, i.uv).bg;
                fixed4 col = tex2D(_MainTex, i.uv + (fc - .5f) / 8.);
                return col;
            }
            ENDCG
        }
    }
}
