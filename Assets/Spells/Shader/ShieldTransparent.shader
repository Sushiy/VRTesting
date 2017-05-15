Shader "Spells/ShieldTransparent"
{
	Properties
	{
		_Color("Color Tint", Color) = (1,1,1,1)
		_MainTex("Noise Texture", 2D) = "white" {}
		//_EmissionColor("Emission Color", Color) = (0,0,0,1)
		_Intensity("Transparency Intensity", Range(0.1, 10.0)) = 1.0
		_AlphaTex("Alpha Texture", 2D) = "black" {}
	}
	
	SubShader
	{

		Tags { "Queue" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"


			struct VertexInput
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 alpha : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 alpha : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			half _Intensity;

			v2f vert(VertexInput v)
			{
				v2f o;

				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.alpha = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) + _Color;
				fixed4 tex = tex2D(_MainTex, _MainTex_ST.xy * i.uv.xy + _MainTex_ST.zw);
				fixed alpha = tex.a * _Color.a;	

				//col *= tex2D(_Emission, i.uv) + _EmissionColor * _Intensity;

				return col;
			}

			ENDCG

		}

	}
		//Fallback "Diffuse"
}

