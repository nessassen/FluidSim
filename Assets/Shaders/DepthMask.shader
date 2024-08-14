 Shader "DepthMask"
 {
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	}
    SubShader
    {
        Tags {"Queue" = "Geometry-1" }
        Lighting Off
        Pass
        {
            ZWrite On
            ZTest LEqual
            ColorMask 0     
        }
    }
 }