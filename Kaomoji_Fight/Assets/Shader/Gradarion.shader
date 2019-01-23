Shader "Custom/Gradarion" {
	Properties{
		_RightColor("Right Color", Color) = (1,1,1,1)
		_LeftColor("Left Color", Color) = (1,1,1,1)
		_RightColorPos("Right Color Pos", Range(0, 1)) = 1 //初期値は1
		_RightColorAmount("Right Color Amount", Range(0, 1)) = 0.5 //初期値は0.5
	}
		SubShader{
		Tags{
		"RenderType" = "Opaque"
		"IgnoreProjector" = "True"
		"Queue" = "Transparent"
	}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		fixed4 _RightColor;
	fixed4 _LeftColor;
	fixed _RightColorPos;
	fixed _RightColorAmount;

	struct appdata {
		half4 vertex : POSITION;
		half2 uv : TEXCOORD0;
	};
	struct v2f {
		half4 vertex : POSITION;
		fixed4 color : COLOR;
		half2 uv : TEXCOORD0;
	};

	v2f vert(appdata v) {
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;

		return o;
	}
	fixed4 frag(v2f i) : COLOR{
		fixed amount = clamp(abs(_RightColorPos - i.uv.y) + (0.5 - _RightColorAmount), 0, 1);
	i.color = lerp(_RightColor, _LeftColor, amount);

	return i.color;
	}
		ENDCG
	}

	}
}
