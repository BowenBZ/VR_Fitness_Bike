Shader "NatureManufacture Shaders/Debug/FlowMapUV4" {
SubShader {
    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        // vertex input: position, second UV
        struct appdata {
            float4 vertex : POSITION;
            float4 texcoord3 : TEXCOORD3;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            float4 uv : TEXCOORD0;
        };
        
        v2f vert (appdata v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex );
            o.uv = float4( v.texcoord3.xy, 0, 0 );
            return o;
        }
        
        half4 frag( v2f i ) : SV_Target {

        	float4 c = float4((i.uv.x+1)*0.5,(i.uv.y+1)*0.5,0,1);
        	//float4 c = float4(0,i.uv.y,0,1);
         	

            return c;
        }
        ENDCG
    }
}
}