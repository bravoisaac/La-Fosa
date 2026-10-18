Shader "LaFosa/Water"
{
    Properties { _Color ("Water", Color) = (.12,.58,.64,.48) }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float3 world : TEXCOORD0; UNITY_FOG_COORDS(1) };
            fixed4 _Color;
            v2f vert(appdata v)
            {
                v2f o; o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
                v.vertex.y += sin(o.world.x * .3 + _Time.y) * .08 + cos(o.world.z * .35 + _Time.y * .8) * .08;
                o.pos = UnityObjectToClipPos(v.vertex); UNITY_TRANSFER_FOG(o, o.pos); return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float wave = sin(i.world.x * .8 + i.world.z * .45 + _Time.y * 1.3);
                fixed4 c = _Color;
                c.rgb += pow(saturate(wave), 24) * .18;
                UNITY_APPLY_FOG(i.fogCoord, c); return c;
            }
            ENDCG
        }
    }
}
