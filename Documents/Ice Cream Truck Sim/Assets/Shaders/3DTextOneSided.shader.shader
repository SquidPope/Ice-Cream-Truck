Shader "Custom/3DTextOneSided.shader" 
{
	Properties
	{
		_mainTex ("Font Texture", 2D) = "white" {}
		_color ("Text Color", Color) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Lighting Off Cull Back ZWrite Off Fog {Mode Off}
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Color [_color]
			SetTexture [_mainTex]
			{
				combine primary, texture * primary
			}
		}
	}
}
