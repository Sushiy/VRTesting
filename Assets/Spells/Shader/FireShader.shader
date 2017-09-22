Shader "Spells/FireShader"
{
	Properties
	{
		_Color ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Noise Texture", 2D) = "white" {}
		_EmissionColor("Emission Color", Color) = (0,0,0,1)
		_Intensity("Emission Intensity", Range(0.1, 10.0)) = 1.0
		_Emission("Emission Map", 2D) = "black" {}
		_Bubble1("Bubbling Value 1", float) = 1.0
		_Bubble2("Bubbling Value 2", float) = 1.0
		_Bubble3("Bubbling Value 3", float) = 1.0
		_TexSpeed("ScrollingSpeed", float) = 2.0
	}
	SubShader
	{

		Tags{ "RenderType" = "Opaque" }
		//Tags { "Queue" = "Transparent"}
		//Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature _EMISSION_MAP

			#include "UnityCG.cginc"


			struct VertexInput
			{
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
				half2 emit : TEXCOORD1;
			};

			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed2 emit : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _Emission;
			half4 _MainTex_ST;
			half4 _Color;
			half4 _EmissionColor;
			float _Intensity;
			float _Bubble1;
			float _Bubble2;
			//Multiplies Speed of Bubbling over Time
			float _Bubble3;
			//Speed of Texture scrolling
			float _TexSpeed;

			half3 GetEmission (VertexInput i) 
			{
				#if defined(FORWARD_BASE_PASS)
					#if defined(_EMISSION_MAP)
						return tex2D(_Emission, i.uv.xy) * _EmissionColor;
					#else
						return _EmissionColor;
					#endif
				#else
					return 0;
				#endif
			}
			

			v2f vert (VertexInput v)
			{
				v2f o;
				// Vertex Animation
				//v.vertex.x += sin ((v.vertex.y + _Time * _Bubble3) * _Bubble2) * _Bubble1;
				v.vertex.xyz += (sin((v.vertex.x + _Time * _Bubble3) * _Bubble2) + cos((v.vertex.z + _Time * _Bubble3) * _Bubble2 )) * _Bubble1;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				// Moving Texture
				o.uv += _Time.xx * _TexSpeed;

				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.emit = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv) + _Color;
				//half4 tex = tex2D(_MainTex, _MainTex_ST.xy * i.uv.xy + _MainTex_ST.zw);
				//half alpha = tex.a * _Color.a;	
				
				col *= tex2D(_Emission, i.uv) + _EmissionColor * _Intensity;

				//Emission
				col.rgb += GetEmission(i);
				
				return col;
			}
			ENDCG
		}
		
	}
	//Fallback "Diffuse"
}
