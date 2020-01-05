// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, alpha blended
// cull off

Shader "tk2d/BlendVertexColorMask" 
{
	Properties 
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_MaskTex("Mask (RGB) Trans (A)", 2D) = "white" {}
		_PartsColor("Parts Color", Color) = (1, 1, 1, 1)
		_Color("Color", Color) = (1, 1, 1, 1)
		[Toggle(_INTERPOLATED_PARTS_COLOR)] _InterpolatedPartsColor("Interpolated Parts Color", Int) = 0
		[Toggle(_INVERT_PARTS_COLOR_MASK)] _InvertPartsColorMask("Invert Parts Color Mask", Int) = 1
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		LOD 110
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature _INVERT_PARTS_COLOR_MASK
			#pragma shader_feature _INTERPOLATED_PARTS_COLOR 
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MaskTex;
			float4 _MainTex_ST;

			fixed4 _Color;
			fixed4 _PartsColor;

			struct vin_vct 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_vct
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * _Color; //메인텍스쳐
				fixed4 mask = tex2D(_MaskTex, i.texcoord); //마스킹 텍스쳐
				#if _INVERT_PARTS_COLOR_MASK //마스크영역의 반전플래그 
						mask.r = 1 - mask.r;
				#endif
				
				#if _INTERPOLATED_PARTS_COLOR
						col.rgb = lerp(col.rgb, _PartsColor.rgb, mask.r);
				#else
						col.rgb = col.rgb * lerp(float3(1, 1, 1), _PartsColor.rgb, mask.r);
				#endif

						col.a = mask.a;

				return col;
			}
			
			ENDCG
		} 
	}
}
