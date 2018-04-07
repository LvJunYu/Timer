// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:5,bsrc:3,bdst:0,culm:2,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32224,y:32735|normal-9-OUT,emission-9-OUT,alpha-10-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:33311,y:32693,ptlb:node_2,ptin:_node_2,tex:2a64b36a642169e429895a82ddf4a31b,ntxv:0,isnm:False|UVIN-4-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33340,y:32872,ptlb:node_3,ptin:_node_3,tex:2a64b36a642169e429895a82ddf4a31b,ntxv:0,isnm:False|UVIN-5-UVOUT;n:type:ShaderForge.SFN_Panner,id:4,x:33534,y:32695,spu:0,spv:-1.5;n:type:ShaderForge.SFN_Panner,id:5,x:33534,y:32872,spu:0,spv:-1.5;n:type:ShaderForge.SFN_Multiply,id:6,x:33090,y:32793|A-2-RGB,B-3-RGB;n:type:ShaderForge.SFN_Multiply,id:7,x:32895,y:32852|A-6-OUT,B-23-OUT;n:type:ShaderForge.SFN_Tex2d,id:8,x:33100,y:33016,ptlb:node_8,ptin:_node_8,tex:47829b6177d61e044b95584b5d8c3058,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9,x:32681,y:32898|A-7-OUT,B-10-RGB;n:type:ShaderForge.SFN_VertexColor,id:10,x:32885,y:33018;n:type:ShaderForge.SFN_Power,id:23,x:33077,y:32935|VAL-8-RGB,EXP-28-OUT;n:type:ShaderForge.SFN_Vector1,id:28,x:33311,y:33067,v1:1.5;proporder:2-3-8;pass:END;sub:END;*/

Shader "Shader Forge/tongtong" {
    Properties {
        _node_2 ("node_2", 2D) = "white" {}
        _node_3 ("node_3", 2D) = "white" {}
        _node_8 ("node_8", 2D) = "white" {}
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
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float4 node_44 = _Time + _TimeEditor;
                float2 node_43 = i.uv0;
                float2 node_4 = (node_43.rg+node_44.g*float2(0,-1.5));
                float2 node_5 = (node_43.rg+node_44.g*float2(0,-1.5));
                float4 node_10 = i.vertexColor;
                float3 node_9 = (((tex2D(_node_2,TRANSFORM_TEX(node_4, _node_2)).rgb*tex2D(_node_3,TRANSFORM_TEX(node_5, _node_3)).rgb)*pow(tex2D(_node_8,TRANSFORM_TEX(node_43.rg, _node_8)).rgb,1.5))*node_10.rgb);
                float3 normalLocal = node_9;
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
////// Lighting:
////// Emissive:
                float3 emissive = node_9;
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,node_10.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
