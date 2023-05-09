// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.19 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.19;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:4013,x:35770,y:32817,varname:node_4013,prsc:2|diff-7925-OUT,emission-8300-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:6172,x:33971,y:32522,ptovrint:False,ptlb:Base Texture (RGBA),ptin:_BaseTextureRGBA,varname:node_6172,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0766eaed102a5d84e842abc31b2fb184,ntxv:1,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2801,x:34443,y:32460,varname:node_2801,prsc:2,tex:0766eaed102a5d84e842abc31b2fb184,ntxv:0,isnm:False|UVIN-8537-UVOUT,TEX-6172-TEX;n:type:ShaderForge.SFN_FragmentPosition,id:4192,x:33317,y:32265,varname:node_4192,prsc:2;n:type:ShaderForge.SFN_Append,id:6407,x:33545,y:32265,varname:node_6407,prsc:2|A-4192-X,B-4192-Z;n:type:ShaderForge.SFN_Multiply,id:2219,x:33818,y:32235,varname:node_2219,prsc:2|A-6407-OUT,B-9962-OUT;n:type:ShaderForge.SFN_Vector1,id:9962,x:33559,y:32208,varname:node_9962,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Color,id:9732,x:34030,y:32969,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:node_9732,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9568627,c2:0.8509804,c3:0,c4:1;n:type:ShaderForge.SFN_Panner,id:8537,x:34116,y:32246,varname:node_8537,prsc:2,spu:0.005,spv:-0.01|UVIN-2219-OUT;n:type:ShaderForge.SFN_Tex2d,id:8764,x:34011,y:32743,ptovrint:False,ptlb:node_8764,ptin:_node_8764,varname:node_8764,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f2995be7f9c3d6f41a1c59290b660ded,ntxv:0,isnm:False|UVIN-6068-UVOUT;n:type:ShaderForge.SFN_Panner,id:6068,x:33824,y:32674,varname:node_6068,prsc:2,spu:-0.02,spv:0.01|UVIN-4109-OUT;n:type:ShaderForge.SFN_Multiply,id:6031,x:34286,y:32672,varname:node_6031,prsc:2|A-9732-RGB,B-8764-R;n:type:ShaderForge.SFN_Add,id:7925,x:34913,y:32903,varname:node_7925,prsc:2|A-6031-OUT,B-2801-RGB;n:type:ShaderForge.SFN_Multiply,id:4109,x:33632,y:32674,varname:node_4109,prsc:2|A-6407-OUT,B-9962-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:8300,x:35250,y:33050,varname:node_8300,prsc:2|IN-7925-OUT,IMIN-5251-OUT,IMAX-4990-OUT,OMIN-4521-OUT,OMAX-305-OUT;n:type:ShaderForge.SFN_Vector1,id:5251,x:35039,y:33102,varname:node_5251,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Vector1,id:4990,x:35039,y:33162,varname:node_4990,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:4521,x:35039,y:33218,varname:node_4521,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:305,x:35039,y:33267,varname:node_305,prsc:2,v1:1;proporder:6172-9732-8764;pass:END;sub:END;*/

Shader "MK4/Lava Glow Mobile" {
    Properties {
        _BaseTextureRGBA ("Base Texture (RGBA)", 2D) = "gray" {}
        _Color2 ("Color2", Color) = (0.9568627,0.8509804,0,1)
        _node_8764 ("node_8764", 2D) = "white" {}
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
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _BaseTextureRGBA; uniform float4 _BaseTextureRGBA_ST;
            uniform float4 _Color2;
            uniform sampler2D _node_8764; uniform float4 _node_8764_ST;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 node_5180 = _Time + _TimeEditor;
                float2 node_6407 = float2(i.posWorld.r,i.posWorld.b);
                float node_9962 = 0.2;
                float2 node_6068 = ((node_6407*node_9962)+node_5180.g*float2(-0.02,0.01));
                float4 _node_8764_var = tex2D(_node_8764,TRANSFORM_TEX(node_6068, _node_8764));
                float2 node_8537 = ((node_6407*node_9962)+node_5180.g*float2(0.005,-0.01));
                float4 node_2801 = tex2D(_BaseTextureRGBA,TRANSFORM_TEX(node_8537, _BaseTextureRGBA));
                float3 node_7925 = ((_Color2.rgb*_node_8764_var.r)+node_2801.rgb);
                float node_5251 = 0.2;
                float node_4521 = 0.0;
                float3 emissive = (node_4521 + ( (node_7925 - node_5251) * (1.0 - node_4521) ) / (1.0 - node_5251));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
