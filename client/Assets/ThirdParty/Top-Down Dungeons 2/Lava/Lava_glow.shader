// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.19 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.19;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:4013,x:35770,y:32817,varname:node_4013,prsc:2|diff-5179-OUT,emission-5009-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:9718,x:31951,y:32437,ptovrint:False,ptlb:Distort Texture,ptin:_DistortTexture,varname:node_9718,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6617,x:32196,y:32197,varname:node_6617,prsc:2,ntxv:0,isnm:False|UVIN-1224-UVOUT,TEX-9718-TEX;n:type:ShaderForge.SFN_Tex2d,id:5110,x:32196,y:32394,varname:node_5110,prsc:2,ntxv:0,isnm:False|UVIN-2577-UVOUT,TEX-9718-TEX;n:type:ShaderForge.SFN_Panner,id:1224,x:31817,y:32197,varname:node_1224,prsc:2,spu:-0.015,spv:0.015|UVIN-2219-OUT;n:type:ShaderForge.SFN_Panner,id:2577,x:31817,y:32395,varname:node_2577,prsc:2,spu:0.015,spv:-0.015|UVIN-5282-OUT;n:type:ShaderForge.SFN_Vector1,id:387,x:31376,y:32628,varname:node_387,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Multiply,id:4308,x:31622,y:32556,varname:node_4308,prsc:2|A-2219-OUT,B-387-OUT;n:type:ShaderForge.SFN_Multiply,id:360,x:31609,y:32761,varname:node_360,prsc:2|A-2219-OUT,B-4521-OUT;n:type:ShaderForge.SFN_Tex2d,id:2270,x:32196,y:32522,varname:node_2270,prsc:2,ntxv:0,isnm:False|UVIN-3255-UVOUT,TEX-9718-TEX;n:type:ShaderForge.SFN_Tex2d,id:8449,x:32196,y:32659,varname:node_8449,prsc:2,ntxv:0,isnm:False|UVIN-4113-UVOUT,TEX-9718-TEX;n:type:ShaderForge.SFN_Panner,id:3255,x:31817,y:32575,varname:node_3255,prsc:2,spu:0.015,spv:0.015|UVIN-4308-OUT;n:type:ShaderForge.SFN_Vector1,id:4521,x:31361,y:32795,varname:node_4521,prsc:2,v1:1.35;n:type:ShaderForge.SFN_Panner,id:4113,x:31823,y:32785,varname:node_4113,prsc:2,spu:-0.015,spv:-0.015|UVIN-360-OUT;n:type:ShaderForge.SFN_Multiply,id:2221,x:32856,y:33792,varname:node_2221,prsc:2|A-9964-T,B-3246-OUT;n:type:ShaderForge.SFN_Sin,id:9030,x:33022,y:33792,varname:node_9030,prsc:2|IN-2221-OUT;n:type:ShaderForge.SFN_RemapRange,id:6595,x:33210,y:33817,varname:node_6595,prsc:2,frmn:-1,frmx:1,tomn:1,tomx:0.6|IN-9030-OUT;n:type:ShaderForge.SFN_Divide,id:2883,x:33372,y:33801,varname:node_2883,prsc:2|A-2446-OUT,B-6595-OUT;n:type:ShaderForge.SFN_Vector1,id:3246,x:32623,y:33857,varname:node_3246,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:5282,x:31622,y:32375,varname:node_5282,prsc:2|A-2219-OUT,B-5504-OUT;n:type:ShaderForge.SFN_Vector1,id:5504,x:31376,y:32516,varname:node_5504,prsc:2,v1:0.85;n:type:ShaderForge.SFN_Add,id:8689,x:33078,y:32343,varname:node_8689,prsc:2|A-6617-R,B-5110-R;n:type:ShaderForge.SFN_Add,id:206,x:33078,y:32544,varname:node_206,prsc:2|A-2270-R,B-8449-R;n:type:ShaderForge.SFN_Add,id:2443,x:33518,y:32973,varname:node_2443,prsc:2|A-8689-OUT,B-206-OUT;n:type:ShaderForge.SFN_Blend,id:8503,x:34374,y:32949,varname:node_8503,prsc:2,blmd:1,clmp:True|SRC-9064-RGB,DST-2443-OUT;n:type:ShaderForge.SFN_Color,id:9064,x:34144,y:33094,ptovrint:False,ptlb:Color1,ptin:_Color1,varname:node_9064,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4411765,c2:0.2099391,c3:0,c4:1;n:type:ShaderForge.SFN_Tex2dAsset,id:6172,x:33971,y:32522,ptovrint:False,ptlb:Base Texture (RGBA),ptin:_BaseTextureRGBA,varname:node_6172,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0766eaed102a5d84e842abc31b2fb184,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2801,x:34443,y:32460,varname:node_2801,prsc:2,tex:0766eaed102a5d84e842abc31b2fb184,ntxv:0,isnm:False|UVIN-8537-UVOUT,TEX-6172-TEX;n:type:ShaderForge.SFN_Lerp,id:362,x:35005,y:32961,varname:node_362,prsc:2|A-1042-OUT,B-9004-OUT,T-2801-A;n:type:ShaderForge.SFN_Add,id:5179,x:35270,y:32787,varname:node_5179,prsc:2|A-362-OUT,B-2801-RGB;n:type:ShaderForge.SFN_Multiply,id:9004,x:34696,y:32815,varname:node_9004,prsc:2|A-2883-OUT,B-2801-RGB;n:type:ShaderForge.SFN_Vector3,id:2446,x:33266,y:33669,varname:node_2446,prsc:2,v1:0.1911765,v2:0.05060554,v3:0.05060554;n:type:ShaderForge.SFN_Time,id:9964,x:32602,y:33682,varname:node_9964,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7738,x:32856,y:34030,varname:node_7738,prsc:2|A-9964-T,B-6928-OUT;n:type:ShaderForge.SFN_Sin,id:1292,x:33022,y:34030,varname:node_1292,prsc:2|IN-7738-OUT;n:type:ShaderForge.SFN_RemapRange,id:6891,x:33210,y:34055,varname:node_6891,prsc:2,frmn:-1,frmx:1,tomn:1,tomx:20|IN-1292-OUT;n:type:ShaderForge.SFN_Vector1,id:6928,x:32623,y:34095,varname:node_6928,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Lerp,id:4508,x:34077,y:33670,varname:node_4508,prsc:2|A-8000-OUT,B-2984-OUT,T-9417-R;n:type:ShaderForge.SFN_Multiply,id:7306,x:31727,y:33515,varname:node_7306,prsc:2|A-2219-OUT,B-7851-OUT;n:type:ShaderForge.SFN_Vector1,id:7851,x:31536,y:33515,varname:node_7851,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Tex2d,id:9417,x:32376,y:33682,varname:node_9417,prsc:2,ntxv:0,isnm:False|UVIN-9439-UVOUT,TEX-9718-TEX;n:type:ShaderForge.SFN_Vector3,id:8000,x:33470,y:33931,varname:node_8000,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Divide,id:2984,x:33380,y:34093,varname:node_2984,prsc:2|A-9732-RGB,B-6891-OUT;n:type:ShaderForge.SFN_Add,id:1998,x:34547,y:33416,varname:node_1998,prsc:2|A-4508-OUT,B-8503-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:4192,x:30756,y:32396,varname:node_4192,prsc:2;n:type:ShaderForge.SFN_Append,id:6407,x:30984,y:32396,varname:node_6407,prsc:2|A-4192-X,B-4192-Z;n:type:ShaderForge.SFN_Multiply,id:2219,x:31253,y:32366,varname:node_2219,prsc:2|A-6407-OUT,B-9962-OUT;n:type:ShaderForge.SFN_Vector1,id:9962,x:31110,y:32487,varname:node_9962,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Panner,id:9439,x:32022,y:33564,varname:node_9439,prsc:2,spu:-0.01,spv:-0.01|UVIN-7306-OUT;n:type:ShaderForge.SFN_Panner,id:6975,x:32022,y:33389,varname:node_6975,prsc:2,spu:0.007,spv:0.007|UVIN-6430-OUT;n:type:ShaderForge.SFN_Lerp,id:4829,x:33744,y:33543,varname:node_4829,prsc:2|A-8000-OUT,B-9732-RGB,T-9378-R;n:type:ShaderForge.SFN_Add,id:1042,x:34794,y:33333,varname:node_1042,prsc:2|A-4829-OUT,B-1998-OUT;n:type:ShaderForge.SFN_Tex2d,id:9378,x:32376,y:33441,varname:node_9378,prsc:2,ntxv:0,isnm:False|UVIN-6975-UVOUT,TEX-9718-TEX;n:type:ShaderForge.SFN_Multiply,id:6430,x:31743,y:33316,varname:node_6430,prsc:2|A-2219-OUT,B-5174-OUT;n:type:ShaderForge.SFN_Vector1,id:5174,x:31533,y:33367,varname:node_5174,prsc:2,v1:0.08;n:type:ShaderForge.SFN_Color,id:9732,x:33069,y:34375,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:node_9732,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9568627,c2:0.8509804,c3:0,c4:1;n:type:ShaderForge.SFN_Panner,id:8537,x:33727,y:32316,varname:node_8537,prsc:2,spu:0.005,spv:-0.01|UVIN-2219-OUT;n:type:ShaderForge.SFN_Slider,id:1787,x:35119,y:33346,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_1787,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Color,id:2225,x:34881,y:33119,ptovrint:False,ptlb:Emission Background,ptin:_EmissionBackground,varname:node_2225,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:5009,x:35494,y:32948,varname:node_5009,prsc:2|A-8000-OUT,B-5179-OUT,T-1787-OUT;n:type:ShaderForge.SFN_Vector3,id:5216,x:35183,y:33222,varname:node_5216,prsc:2,v1:0,v2:0,v3:0;proporder:6172-9718-9064-9732-1787;pass:END;sub:END;*/

Shader "MK4/Lava Glow" {
    Properties {
        _BaseTextureRGBA ("Base Texture (RGBA)", 2D) = "gray" {}
        _DistortTexture ("Distort Texture", 2D) = "white" {}
        _Color1 ("Color1", Color) = (0.4411765,0.2099391,0,1)
        _Color2 ("Color2", Color) = (0.9568627,0.8509804,0,1)
        _Emission ("Emission", Range(0, 1)) = 0.8
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _DistortTexture; uniform float4 _DistortTexture_ST;
            uniform float4 _Color1;
            uniform sampler2D _BaseTextureRGBA; uniform float4 _BaseTextureRGBA_ST;
            uniform float4 _Color2;
            uniform float _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 node_8000 = float3(0,0,0);
                float4 node_8962 = _Time + _TimeEditor;
                float2 node_2219 = (float2(i.posWorld.r,i.posWorld.b)*0.2);
                float2 node_6975 = ((node_2219*0.08)+node_8962.g*float2(0.007,0.007));
                float4 node_9378 = tex2D(_DistortTexture,TRANSFORM_TEX(node_6975, _DistortTexture));
                float4 node_9964 = _Time + _TimeEditor;
                float2 node_9439 = ((node_2219*0.1)+node_8962.g*float2(-0.01,-0.01));
                float4 node_9417 = tex2D(_DistortTexture,TRANSFORM_TEX(node_9439, _DistortTexture));
                float2 node_1224 = (node_2219+node_8962.g*float2(-0.015,0.015));
                float4 node_6617 = tex2D(_DistortTexture,TRANSFORM_TEX(node_1224, _DistortTexture));
                float2 node_2577 = ((node_2219*0.85)+node_8962.g*float2(0.015,-0.015));
                float4 node_5110 = tex2D(_DistortTexture,TRANSFORM_TEX(node_2577, _DistortTexture));
                float2 node_3255 = ((node_2219*0.7)+node_8962.g*float2(0.015,0.015));
                float4 node_2270 = tex2D(_DistortTexture,TRANSFORM_TEX(node_3255, _DistortTexture));
                float2 node_4113 = ((node_2219*1.35)+node_8962.g*float2(-0.015,-0.015));
                float4 node_8449 = tex2D(_DistortTexture,TRANSFORM_TEX(node_4113, _DistortTexture));
                float2 node_8537 = (node_2219+node_8962.g*float2(0.005,-0.01));
                float4 node_2801 = tex2D(_BaseTextureRGBA,TRANSFORM_TEX(node_8537, _BaseTextureRGBA));
                float3 node_5179 = (lerp((lerp(node_8000,_Color2.rgb,node_9378.r)+(lerp(node_8000,(_Color2.rgb/(sin((node_9964.g*0.5))*9.5+10.5)),node_9417.r)+saturate((_Color1.rgb*((node_6617.r+node_5110.r)+(node_2270.r+node_8449.r)))))),((float3(0.1911765,0.05060554,0.05060554)/(sin((node_9964.g*1.0))*-0.2+0.8))*node_2801.rgb),node_2801.a)+node_2801.rgb);
                float3 diffuseColor = node_5179;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = lerp(node_8000,node_5179,_Emission);
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _DistortTexture; uniform float4 _DistortTexture_ST;
            uniform float4 _Color1;
            uniform sampler2D _BaseTextureRGBA; uniform float4 _BaseTextureRGBA_ST;
            uniform float4 _Color2;
            uniform float _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 node_8000 = float3(0,0,0);
                float4 node_6111 = _Time + _TimeEditor;
                float2 node_2219 = (float2(i.posWorld.r,i.posWorld.b)*0.2);
                float2 node_6975 = ((node_2219*0.08)+node_6111.g*float2(0.007,0.007));
                float4 node_9378 = tex2D(_DistortTexture,TRANSFORM_TEX(node_6975, _DistortTexture));
                float4 node_9964 = _Time + _TimeEditor;
                float2 node_9439 = ((node_2219*0.1)+node_6111.g*float2(-0.01,-0.01));
                float4 node_9417 = tex2D(_DistortTexture,TRANSFORM_TEX(node_9439, _DistortTexture));
                float2 node_1224 = (node_2219+node_6111.g*float2(-0.015,0.015));
                float4 node_6617 = tex2D(_DistortTexture,TRANSFORM_TEX(node_1224, _DistortTexture));
                float2 node_2577 = ((node_2219*0.85)+node_6111.g*float2(0.015,-0.015));
                float4 node_5110 = tex2D(_DistortTexture,TRANSFORM_TEX(node_2577, _DistortTexture));
                float2 node_3255 = ((node_2219*0.7)+node_6111.g*float2(0.015,0.015));
                float4 node_2270 = tex2D(_DistortTexture,TRANSFORM_TEX(node_3255, _DistortTexture));
                float2 node_4113 = ((node_2219*1.35)+node_6111.g*float2(-0.015,-0.015));
                float4 node_8449 = tex2D(_DistortTexture,TRANSFORM_TEX(node_4113, _DistortTexture));
                float2 node_8537 = (node_2219+node_6111.g*float2(0.005,-0.01));
                float4 node_2801 = tex2D(_BaseTextureRGBA,TRANSFORM_TEX(node_8537, _BaseTextureRGBA));
                float3 node_5179 = (lerp((lerp(node_8000,_Color2.rgb,node_9378.r)+(lerp(node_8000,(_Color2.rgb/(sin((node_9964.g*0.5))*9.5+10.5)),node_9417.r)+saturate((_Color1.rgb*((node_6617.r+node_5110.r)+(node_2270.r+node_8449.r)))))),((float3(0.1911765,0.05060554,0.05060554)/(sin((node_9964.g*1.0))*-0.2+0.8))*node_2801.rgb),node_2801.a)+node_2801.rgb);
                float3 diffuseColor = node_5179;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
