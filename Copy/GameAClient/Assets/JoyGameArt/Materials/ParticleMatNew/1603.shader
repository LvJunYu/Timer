// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:5,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32372,y:32705|emission-184-OUT,alpha-183-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:33227,y:32606,ptlb:node_2,ptin:_node_2,tex:90d746ff86627fa46b1c3cf2cb483897,ntxv:0,isnm:False|UVIN-4-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33242,y:32799,ptlb:node_3,ptin:_node_3,tex:9697c933d4fe97c4fb54420cabba767d,ntxv:0,isnm:False|UVIN-5-UVOUT;n:type:ShaderForge.SFN_Panner,id:4,x:33435,y:32606,spu:-0.08,spv:0.03;n:type:ShaderForge.SFN_Panner,id:5,x:33435,y:32799,spu:0.03,spv:0.1;n:type:ShaderForge.SFN_Multiply,id:6,x:33043,y:32659|A-2-R,B-3-R;n:type:ShaderForge.SFN_Vector3,id:7,x:33043,y:32834,v1:0.2,v2:1,v3:1;n:type:ShaderForge.SFN_Multiply,id:8,x:32893,y:32716|A-6-OUT,B-7-OUT;n:type:ShaderForge.SFN_Tex2d,id:92,x:33261,y:32996,ptlb:node_92,ptin:_node_92,tex:fcf02fe29aab0eb408ae8563bfa42b8b,ntxv:0,isnm:False|UVIN-93-UVOUT;n:type:ShaderForge.SFN_Panner,id:93,x:33435,y:32996,spu:0.1,spv:0.09;n:type:ShaderForge.SFN_Tex2d,id:94,x:33261,y:33195,ptlb:node_94,ptin:_node_94,tex:a9e30cfde75b96240827c4da498d6cee,ntxv:0,isnm:False|UVIN-95-UVOUT;n:type:ShaderForge.SFN_Panner,id:95,x:33435,y:33184,spu:0.12,spv:-0.05;n:type:ShaderForge.SFN_Multiply,id:96,x:33069,y:32996|A-92-RGB,B-94-RGB;n:type:ShaderForge.SFN_Vector3,id:97,x:33055,y:33195,v1:0.1,v2:1,v3:2;n:type:ShaderForge.SFN_Multiply,id:98,x:32911,y:32973|A-96-OUT,B-97-OUT;n:type:ShaderForge.SFN_Add,id:99,x:32809,y:32853|A-8-OUT,B-98-OUT;n:type:ShaderForge.SFN_Fresnel,id:140,x:32809,y:33090|EXP-150-OUT;n:type:ShaderForge.SFN_Multiply,id:141,x:32673,y:32909|A-99-OUT,B-140-OUT;n:type:ShaderForge.SFN_Vector1,id:150,x:32892,y:33258,v1:1.7;n:type:ShaderForge.SFN_VertexColor,id:183,x:32689,y:33128;n:type:ShaderForge.SFN_Multiply,id:184,x:32615,y:33020|A-141-OUT,B-183-RGB;proporder:2-3-92-94;pass:END;sub:END;*/

Shader "Shader Forge/1603" {
    Properties {
        _node_2 ("node_2", 2D) = "white" {}
        _node_3 ("node_3", 2D) = "white" {}
        _node_92 ("node_92", 2D) = "white" {}
        _node_94 ("node_94", 2D) = "white" {}
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
            Blend One One
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
            uniform sampler2D _node_92; uniform float4 _node_92_ST;
            uniform sampler2D _node_94; uniform float4 _node_94_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_196 = _Time + _TimeEditor;
                float2 node_195 = i.uv0;
                float2 node_4 = (node_195.rg+node_196.g*float2(-0.08,0.03));
                float2 node_5 = (node_195.rg+node_196.g*float2(0.03,0.1));
                float2 node_93 = (node_195.rg+node_196.g*float2(0.1,0.09));
                float2 node_95 = (node_195.rg+node_196.g*float2(0.12,-0.05));
                float4 node_183 = i.vertexColor;
                float3 emissive = (((((tex2D(_node_2,TRANSFORM_TEX(node_4, _node_2)).r*tex2D(_node_3,TRANSFORM_TEX(node_5, _node_3)).r)*float3(0.2,1,1))+((tex2D(_node_92,TRANSFORM_TEX(node_93, _node_92)).rgb*tex2D(_node_94,TRANSFORM_TEX(node_95, _node_94)).rgb)*float3(0.1,1,2)))*pow(1.0-max(0,dot(normalDirection, viewDirection)),1.7))*node_183.rgb);
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,node_183.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
