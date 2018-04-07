// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:5,bsrc:3,bdst:0,culm:2,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32060,y:32844|emission-17-OUT,alpha-16-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:33408,y:32668,ptlb:node_2,ptin:_node_2,tex:0aadd7b77bb616f43a7529dbda5ba639,ntxv:0,isnm:False|UVIN-4-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33418,y:32842,ptlb:node_3,ptin:_node_3,tex:90d746ff86627fa46b1c3cf2cb483897,ntxv:0,isnm:False|UVIN-5-UVOUT;n:type:ShaderForge.SFN_Panner,id:4,x:33575,y:32688,spu:0,spv:1.8;n:type:ShaderForge.SFN_Panner,id:5,x:33564,y:32849,spu:0,spv:1.6;n:type:ShaderForge.SFN_Multiply,id:6,x:33174,y:32792|A-2-RGB,B-3-RGB;n:type:ShaderForge.SFN_Multiply,id:7,x:33022,y:32885|A-6-OUT,B-8-RGB;n:type:ShaderForge.SFN_Tex2d,id:8,x:33243,y:32982,ptlb:node_8,ptin:_node_8,tex:11c0e37b41858cc49911792f3805fa26,ntxv:0,isnm:False|UVIN-9-UVOUT;n:type:ShaderForge.SFN_Panner,id:9,x:33439,y:33002,spu:0,spv:2;n:type:ShaderForge.SFN_Multiply,id:10,x:32784,y:32885|A-7-OUT,B-12-OUT;n:type:ShaderForge.SFN_Tex2d,id:11,x:33247,y:33313,ptlb:node_11,ptin:_node_11,tex:5eefb7e3a155c01479a14271481152e6,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Power,id:12,x:33033,y:33103|VAL-11-RGB,EXP-13-OUT;n:type:ShaderForge.SFN_Vector1,id:13,x:33458,y:33206,v1:1.3;n:type:ShaderForge.SFN_Vector3,id:14,x:32809,y:33055,v1:1,v2:0.4344828,v3:0;n:type:ShaderForge.SFN_Multiply,id:15,x:32587,y:32824|A-10-OUT,B-14-OUT;n:type:ShaderForge.SFN_VertexColor,id:16,x:32666,y:33118;n:type:ShaderForge.SFN_Multiply,id:17,x:32303,y:32957|A-80-OUT,B-16-RGB;n:type:ShaderForge.SFN_Multiply,id:80,x:32404,y:32762|A-85-OUT,B-10-OUT;n:type:ShaderForge.SFN_Vector1,id:85,x:32719,y:32679,v1:2;proporder:2-3-8-11;pass:END;sub:END;*/

Shader "Shader Forge/luoxuanxian" {
    Properties {
        _node_2 ("node_2", 2D) = "white" {}
        _node_3 ("node_3", 2D) = "white" {}
        _node_8 ("node_8", 2D) = "white" {}
        _node_11 ("node_11", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _node_2; uniform float4 _node_2_ST;
            uniform sampler2D _node_3; uniform float4 _node_3_ST;
            uniform sampler2D _node_8; uniform float4 _node_8_ST;
            uniform sampler2D _node_11; uniform float4 _node_11_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_103 = _Time + _TimeEditor;
                float2 node_102 = i.uv0;
                float2 node_4 = (node_102.rg+node_103.g*float2(0,1.8));
                float2 node_5 = (node_102.rg+node_103.g*float2(0,1.6));
                float2 node_9 = (node_102.rg+node_103.g*float2(0,2));
                float3 node_10 = (((tex2D(_node_2,TRANSFORM_TEX(node_4, _node_2)).rgb*tex2D(_node_3,TRANSFORM_TEX(node_5, _node_3)).rgb)*tex2D(_node_8,TRANSFORM_TEX(node_9, _node_8)).rgb)*pow(tex2D(_node_11,TRANSFORM_TEX(node_102.rg, _node_11)).rgb,1.3));
                float4 node_16 = i.vertexColor;
                float3 emissive = ((2.0*node_10)*node_16.rgb);
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,node_16.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
