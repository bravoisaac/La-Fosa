Shader "LaFosa/Water"
{
    Properties
    {
        _Color ("Agua costera", Color) = (.045,.28,.29,1)
        _TrenchColor ("Agua de la fosa", Color) = (.018,.12,.15,1)
        _SurfaceColor ("Mar visto desde fuera", Color) = (.018,.12,.15,1)
        _ShoreOpacity ("Turbidez junto a la orilla", Range(0,1)) = .65
        _SeaOpacity ("Turbidez uniforme del mar", Range(0,1)) = .85
        _ShoreFade ("Orilla Z y final de transición Z", Vector) = (-3,17,0,0)
        _Trench ("Centro X/Z y radios interior/exterior", Vector) = (0,75,23,31)
        _WaterLevel ("Nivel del agua", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        // Composite a softened view through the water, without blurring the HUD or land.
        GrabPass { "_LaFosaWaterBackground" }
        Blend Off
        ZWrite On
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float3 world : TEXCOORD0; float4 grab : TEXCOORD1; UNITY_FOG_COORDS(2) };
            fixed4 _Color, _TrenchColor, _SurfaceColor;
            float4 _Trench, _ShoreFade, _LaFosaWaterBackground_TexelSize;
            float _ShoreOpacity, _SeaOpacity, _WaterLevel;
            sampler2D _LaFosaWaterBackground;
            v2f vert(appdata v)
            {
                v2f o; o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
                v.vertex.y += sin(o.world.x * .3 + _Time.y) * .08 + cos(o.world.z * .35 + _Time.y * .8) * .08;
                o.pos = UnityObjectToClipPos(v.vertex); o.grab = ComputeGrabScreenPos(o.pos); UNITY_TRANSFER_FOG(o, o.pos); return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float wave = sin(i.world.x * .8 + i.world.z * .45 + _Time.y * 1.3);
                float trench = 1 - smoothstep(_Trench.z, _Trench.w, distance(i.world.xz, _Trench.xy));
                float above = step(_WaterLevel - .15, _WorldSpaceCameraPos.y);
                // Only the shoreline changes surface clarity; the trench tint stays underwater.
                float offshore = smoothstep(_ShoreFade.x, _ShoreFade.y, i.world.z);
                float2 uv = i.grab.xy / i.grab.w;
                float2 texel = abs(_LaFosaWaterBackground_TexelSize.xy);
                float2 radius = texel * lerp(3.5, 4.5, offshore) * (_ScreenParams.y / 900) * above;
                fixed3 behind = tex2D(_LaFosaWaterBackground, saturate(uv)).rgb * .28;
                behind += tex2D(_LaFosaWaterBackground, saturate(uv + float2(radius.x, 0))).rgb * .18;
                behind += tex2D(_LaFosaWaterBackground, saturate(uv - float2(radius.x, 0))).rgb * .18;
                behind += tex2D(_LaFosaWaterBackground, saturate(uv + float2(0, radius.y))).rgb * .18;
                behind += tex2D(_LaFosaWaterBackground, saturate(uv - float2(0, radius.y))).rgb * .18;
                float grazing = 1 - saturate(abs(normalize(_WorldSpaceCameraPos.xyz - i.world).y));
                float opacity = lerp(.3, saturate(lerp(_ShoreOpacity, _SeaOpacity, offshore) + grazing * .04), above);
                fixed3 tint = lerp(lerp(_Color.rgb, _TrenchColor.rgb, trench), _SurfaceColor.rgb, above);
                tint += pow(saturate(wave), 24) * .055;
                fixed4 c = fixed4(lerp(behind, tint, opacity), 1);
                UNITY_APPLY_FOG(i.fogCoord, c); return c;
            }
            ENDCG
        }
    }
}
