Shader "Spells/ShieldTransparent"
{
	Properties
	{
		_MainTex("Noise Texture", 2D) = "white" {}
		_TexColor("Texture Color", Color) = (1.0,1.0,1.0,1.0)
		_RimColor("Rim Color", Color) = (1.0,1.0,1.0,1.0)
		_Shine("Shine", Float) = 10
	}
	
	SubShader
	{

		Tags { "Queue" = "Transparent" "LightMode" = "ForwardBase" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{

			//Cull Off
			zWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"


			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _RimColor;
			uniform fixed4 _TexColor;
			uniform fixed _Shine;


			uniform float4 _LightColor0;



			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float2 alpha : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 alpha : TEXCOORD1;
				float4 posWorld : TEXCOORD2;
				float3 normalDir : TEXCOORD3;
			};

			v2f vert(VertexInput v)
			{
				v2f o;
				o.vertex= UnityObjectToClipPos(v.vertex);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalDir = normalize(mul(half4(v.normal, 0.0), unity_WorldToObject).xyz);
				o.uv = UnityObjectToClipPos(v.vertex);
				o.alpha = v.uv;

				// Moving Texture
				o.uv += _Time.xx;

				return o;
			}



			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 normalDirection = i.normalDir;
				fixed3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				float atten = 1.0;
				
				//lighting
				fixed3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection));
				fixed3 specularReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection)) * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shine);

				//Rim Lighting
				fixed4 white = fixed4(1.0f,1.0f,1.0f,1.0f);
				fixed rim = saturate(dot(normalize(viewDirection), normalDirection));
				fixed3 rimLighting = rim * _TexColor;
				fixed3 lightFinal = rimLighting + diffuseReflection + specularReflection + UNITY_LIGHTMODEL_AMBIENT.rgb;

				//Texture (Stringy works best!)
				fixed4 tex = tex2D(_MainTex, _MainTex_ST.xy * i.uv.xy + _MainTex_ST.zw) + _RimColor;


				//Alpha Map
				fixed alphaT = tex.a + _RimColor.a;
				fixed alphaR = 1 -(rim * _TexColor.a);

				return fixed4(lightFinal * tex, alphaT * alphaR);
			}

			ENDCG

		}

	}
		//Fallback "Diffuse"
}

