Shader "Unlit/InvisibleMask"
{
    SubShader {
		// transparent things.
 
		Tags {"Queue" = "Geometry+10" }
 
		ColorMask 0
		ZWrite On
 
		Pass {}
	}
}
