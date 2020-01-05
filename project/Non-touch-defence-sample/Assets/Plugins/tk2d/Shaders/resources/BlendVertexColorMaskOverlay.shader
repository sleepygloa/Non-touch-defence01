// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, alpha blended
// cull off, overlay

Shader "tk2d/BlendVertexColorMaskOverlay"
{
	Properties
	{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_MaskTex("Mask (RGB) Trans (A)", 2D) = "white" {}
		_PartsColor("Parts Color", Color) = (1, 1, 1, 1)
		_Color("Color", Color) = (1, 1, 1, 1)
		_CutOff("CutOff", Range(0.0, 1.0)) = 0.5
		//_Luminous("Luminous", Range(0.0, 1.0)) = 0.7
		
		[Toggle(_INVERT_PARTS_COLOR_MASK)] _InvertPartsColorMask("Invert Parts Color Mask", Int) = 1
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off Lighting Off Cull Off Fog{ Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		LOD 110
		Alphatest Greater[_Cutoff]
		AlphaToMask True


		Pass
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature _INVERT_PARTS_COLOR_MASK

			#include "UnityCG.cginc"


			sampler2D _MainTex;
			sampler2D _MaskTex;
			float4 _MainTex_ST;
			float _CutOff;
			//float _Luminous;
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

			half OverlayBlendChannel(half base, half blend)
			{
				if (blend < 0.5)
					return saturate(2 * base * blend);
				else
					return saturate(1 - 2 * (1 - base) * (1 - blend));
			}

			half3 OverlayBlend(half3 base, half3 blend)
			{
				return half3(OverlayBlendChannel(base.r, blend.r),
				OverlayBlendChannel(base.g, blend.g),
				OverlayBlendChannel(base.b, blend.b)); // case 1
				//return base * blend; // case 2
			}

			fixed4 frag_mult(v2f_vct i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * _Color; //메인텍스쳐
				fixed4 mask = tex2D(_MaskTex, i.texcoord); //마스킹 텍스쳐
			#if _INVERT_PARTS_COLOR_MASK //마스크영역의 반전플래그 
				mask.r = 1 - mask.r;
			#endif

				col.rgb = col.rgb * lerp(float3(1, 1, 1), _PartsColor.rgb, mask.r);
				if (mask.r > _CutOff)
					col.rgb = OverlayBlend(i.color, col.rgb);
				//else
					//col.rgb = HalfOverlayBlend(i.color, col.rgb);

				if (mask.r > _CutOff)
					col.a = mask.a * _PartsColor.a;
				else
					col.a = mask.a;// *_Color.a;

				return col;
			}

			ENDCG
		}
	}
		Fallback "VertexLit"
}
