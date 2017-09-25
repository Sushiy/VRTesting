
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Spells/FireShader Bump"
{
		Properties {
		_Color ("Color Tint", Color) = (1.0,1.0,1.0,1.0)
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		_EmissionColor("Emission Color", Color) = (0,0,0,1)
		_Intensity("Emission Intensity", Range(0.1, 10.0)) = 1.0
		_Emission("Emission Map", 2D) = "black" {}
		_BumpMap ("Normal Texture", 2D) = "bump" {}
		_BumpDepth ("Bump Depth", Range(-4.0,4.0)) = 1
		_SpecColor ("Specular Color", Color) = (1.0,1.0,1.0,1.0)
		_Shininess ("Shininess", float) = 10
		_RimColor ("Rim Color", Color) = (1.0,1.0,1.0,1.0)
		_RimPower ("Rim Power", Range(0.1,10.0)) = 3.0
		_Bubble1("Bubbling Intensity", float) = 1.0
		_Bubble2("Bubbling Value 2", float) = 1.0
		_Bubble3("Bubbling Speed", float) = 1.0
		_TexSpeed("ScrollingSpeed", float) = 2.0
	}
	SubShader {
		Pass {
			Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers flash
			
			//user defined variables
			uniform sampler2D _MainTex;
			uniform half4 _MainTex_ST;
			uniform sampler2D _Emission;
			uniform half4 _Emission_ST;
			uniform sampler2D _BumpMap;
			uniform half4 _BumpMap_ST;
			uniform half4 _Color;
			uniform half4 _EmissionColor;
			uniform half4 _SpecColor;
			uniform half4 _RimColor;
			uniform half _Shininess;
			uniform half _RimPower;
			uniform half _BumpDepth;
			uniform half _Intensity;

			//Bubble Intensity
			uniform half _Bubble1;
			uniform half _Bubble2;
			//Multiplies Speed of Bubbling over Time
			uniform half _Bubble3;
			//Speed of Texture scrolling
			uniform half _TexSpeed;
			
			//unity defined variables
			uniform half4 _LightColor0;


			
			//base input structs
			struct vertexInput
			{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
				half4 texcoord : TEXCOORD0;
				half2 emit : TEXCOORD1;
				half4 tangent : TANGENT;
			};

			struct vertexOutput
			{
				half4 pos : SV_POSITION;
				half2 tex : TEXCOORD0;
				fixed2 emit : TEXCOORD1;
				half4 posWorld : TEXCOORD2;
				half3 normalWorld : TEXCOORD3;
				half3 tangentWorld : TEXCOORD4;
				half3 binormalWorld : TEXCOORD5;
				
			};
			
			//vertex Function
			
			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;

				v.vertex.xyz += (sin((v.vertex.x + _Time * _Bubble3) * _Bubble2) + cos((v.vertex.z + _Time * _Bubble3) * _Bubble2 )) * _Bubble1;
				//o.texcoord = TRANSFORM_TEX(v.tex, _MainTex);
				o.tex = v.texcoord;
				
				o.normalWorld = normalize( mul( half4( v.normal, 0.0 ), unity_WorldToObject ).xyz );
				o.tangentWorld = normalize( mul( unity_ObjectToWorld, v.tangent ).xyz );
				o.binormalWorld = normalize( cross(o.normalWorld, o.tangentWorld) * v.tangent.w );
				

				o.tex += _Time.xx * _TexSpeed;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.emit = UnityObjectToClipPos(v.vertex);
				
				return o;
			}
			
			//fragment function
			
			half4 frag(vertexOutput i) : COLOR
			{
				half3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - i.posWorld.xyz );
				half3 lightDirection;
				half atten;
				
				if(_WorldSpaceLightPos0.w == 0.0)
				{ 
					//directional light
					atten = 1.0;
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				}

				else{
					half3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
					half distance = length(fragmentToLightSource);
					atten = 1.0/distance;
					lightDirection = normalize(fragmentToLightSource);
				}

				//Texture Maps
				half4 tex = tex2D(_MainTex, i.tex.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				half4 texN = tex2D(_BumpMap, i.tex.xy * _BumpMap_ST.xy + _BumpMap_ST.zw);

				//unpackNormal function
				half3 localCoords = half3(2.0 * texN.ag - half2(1.0, 1.0), 0.0);
				localCoords.z = _BumpDepth;
				
				//normal transpose matrix
				half3x3 local2WorldTranspose = half3x3(
					i.tangentWorld,
					i.binormalWorld,
					i.normalWorld
				);
				
				//calculate normal direction
				half3 normalDirection = normalize( mul( localCoords, local2WorldTranspose ) );
				
				//Lighting
				half3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection));
				half3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDirection)) , _Shininess);
				
				//Rim Lighting
				half rim = 1 - saturate(dot(viewDirection, normalDirection));
				half3 rimLighting = saturate( dot( normalDirection, lightDirection ) * _RimColor.xyz * _LightColor0.xyz * pow( rim, _RimPower ) );
				
				half3 lightFinal = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuseReflection + specularReflection + rimLighting;
				
				half4 col = half4(tex.xyz *lightFinal * _Color.xyz, 1.0);
				col *= _EmissionColor * _Intensity;

				return col;
			}
			
			ENDCG
	}
	}
//	FallBack "Diffuse"
}
