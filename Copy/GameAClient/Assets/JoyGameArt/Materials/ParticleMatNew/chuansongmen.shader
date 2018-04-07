// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:5,bsrc:3,bdst:0,culm:2,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32195,y:32772|emission-442-OUT,alpha-441-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:33477,y:32691,ptlb:node_2,ptin:_node_2,tex:0aadd7b77bb616f43a7529dbda5ba639,ntxv:0,isnm:False|UVIN-5-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33469,y:32861,ptlb:node_3,ptin:_node_3,tex:90d746ff86627fa46b1c3cf2cb483897,ntxv:0,isnm:False|UVIN-6-UVOUT;n:type:ShaderForge.SFN_Multiply,id:4,x:33240,y:32744|A-2-RGB,B-3-RGB;n:type:ShaderForge.SFN_Panner,id:5,x:33647,y:32657,spu:-0.1,spv:0.1;n:type:ShaderForge.SFN_Panner,id:6,x:33642,y:32855,spu:0.1,spv:0.3;n:type:ShaderForge.SFN_Tex2d,id:29,x:33326,y:32986,ptlb:node_29,ptin:_node_29,tex:0c8cf53c9619d504e83c6e83d9a69b40,ntxv:0,isnm:False|UVIN-188-UVOUT;n:type:ShaderForge.SFN_Add,id:57,x:32874,y:32845|A-325-OUT,B-170-OUT;n:type:ShaderForge.SFN_Multiply,id:170,x:33107,y:33047|A-29-B,B-171-OUT;n:type:ShaderForge.SFN_Vector3,id:171,x:33370,y:33184,v1:2,v2:1,v3:0.1;n:type:ShaderForge.SFN_Panner,id:188,x:33542,y:33038,spu:0.6,spv:0.6;n:type:ShaderForge.SFN_Multiply,id:251,x:33129,y:32633|A-252-OUT,B-4-OUT;n:type:ShaderForge.SFN_Vector3,id:252,x:33362,y:32633,v1:0.7794118,v2:0.2138245,v3:0.005730974;n:type:ShaderForge.SFN_Multiply,id:325,x:32973,y:32593|A-326-OUT,B-251-OUT;n:type:ShaderForge.SFN_Vector1,id:326,x:33177,y:32555,v1:3;n:type:ShaderForge.SFN_Multiply,id:423,x:32675,y:32876|A-57-OUT,B-453-OUT;n:type:ShaderForge.SFN_Tex2d,id:424,x:32901,y:33072,ptlb:node_424,ptin:_node_424,tex:5eefb7e3a155c01479a14271481152e6,ntxv:0,isnm:False;n:type:ShaderForge.SFN_VertexColor,id:441,x:32562,y:33147;n:type:ShaderForge.SFN_Multiply,id:442,x:32438,y:32983|A-423-OUT,B-441-RGB;n:type:ShaderForge.SFN_Power,id:453,x:32730,y:33083|VAL-424-RGB,EXP-456-OUT;n:type:ShaderForge.SFN_Vector1,id:456,x:32901,y:33255,v1:1.2;proporder:2-3-29-424;pass:END;sub:END;*/

Shader "Shader Forge/chuansongmen" {
    Properties {
        _node_2 ("node_2", 2D) = "white" {}
        _node_3 ("node_3", 2D) = "white" {}
        _node_29 ("node_29", 2D) = "white" {}
        _node_424 ("node_424", 2D) = "white" {}
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
            uniform sampler2D _node_29; uniform float4 _node_29_ST;
            uniform sampler2D _node_424; uniform float4 _node_424_ST;
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
                float4 node_468 = _Time + _TimeEditor;
                float2 node_467 = i.uv0;
                float2 node_5 = (node_467.rg+node_468.g*float2(-0.1,0.1));
                float2 node_6 = (node_467.rg+node_468.g*float2(0.1,0.3));
                float2 node_188 = (node_467.rg+node_468.g*float2(0.6,0.6));
                float4 node_441 = i.vertexColor;
                float3 emissive = ((((3.0*(float3(0.7794118,0.2138245,0.005730974)*(tex2D(_node_2,TRANSFORM_TEX(node_5, _node_2)).rgb*tex2D(_node_3,TRANSFORM_TEX(node_6, _node_3)).rgb)))+(tex2D(_node_29,TRANSFORM_TEX(node_188, _node_29)).b*float3(2,1,0.1)))*pow(tex2D(_node_424,TRANSFORM_TEX(node_467.rg, _node_424)).rgb,1.2))*node_441.rgb);
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,node_441.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
