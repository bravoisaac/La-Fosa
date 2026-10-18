Shader "LaFosa/Fish"
{
    Properties
    {
        _Length ("Body length", Float) = 1
        _Swim ("Tail movement", Float) = .03
        _Speed ("Tail speed", Float) = 3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; fixed4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; fixed4 color : COLOR; UNITY_FOG_COORDS(0) };
            float _Length, _Swim, _Speed;
            v2f vert(appdata v)
            {
                v2f o;
                float tail = saturate(.35 - v.vertex.z / max(_Length, .01));
                v.vertex.x += sin(_Time.y * _Speed + v.vertex.z * 5 / max(_Length, .01) + unity_ObjectToWorld._m03) * tail * tail * _Swim;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 n = UnityObjectToWorldNormal(v.normal);
                float light = max(0, dot(n, _WorldSpaceLightPos0.xyz));
                o.color.rgb = v.color.rgb * (UNITY_LIGHTMODEL_AMBIENT.rgb + _LightColor0.rgb * (.3 + .7 * light)) + v.color.rgb * v.color.a * .65;
                o.color.a = 1;
                UNITY_TRANSFER_FOG(o, o.pos); return o;
            }
            fixed4 frag(v2f i) : SV_Target { fixed4 c = i.color; UNITY_APPLY_FOG(i.fogCoord, c); return c; }
            ENDCG
        }
    }
}
