Shader "Custom/StencilOnly"
{
	SubShader
	{
		Tags{ "Queue" = "Geometry-1" }  // Write to the stencil buffer before drawing any geometry to the screen
		ColorMask 0 // Don't write to any colour channels
		ZWrite Off // Don't write to the Depth buffer
				   // Write the value 1 to the stencil buffer
		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
		}
		CGINCLUDE
		struct appdata {
		float4 vertex : POSITION;
	};
	struct v2f {
		float4 pos : SV_POSITION;
	};
	v2f vert(appdata v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	half4 frag(v2f i) : SV_Target{
		return half4(1,1,0,1);
	}
		ENDCG

		Pass {
			ZTest Less

	}
	}
}
