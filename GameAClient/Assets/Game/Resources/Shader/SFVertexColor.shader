Shader "AkeShader/SFShaderTest/SFVertexColor"
{
	Properties
	{
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent-10" }
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd alpha:blend


		struct Input
		{
			float4 vertColor;
		};

		void vert(inout appdata_full v, out Input o)
		{
			o.vertColor = v.color;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = IN.vertColor.rgb;
			o.Alpha = IN.vertColor.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
