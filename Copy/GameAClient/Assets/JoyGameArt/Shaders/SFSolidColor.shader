Shader "AkeShader/SFShaderTest/SFSolidColor"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent-10" }
		
		CGPROGRAM
		#pragma surface surf Lambert  noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd alpha:blend


		struct Input
		{
			float4 vertColor;
		};
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
