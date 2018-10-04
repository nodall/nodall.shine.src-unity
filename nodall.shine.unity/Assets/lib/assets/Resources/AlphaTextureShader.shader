Shader "Unlit/AlphaTextureShader"
{
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0.5)
        _SpecColor ("Spec Color", Color) = (1,1,1,0)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range (0.1, 1)) = 0.7
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _AlphaTex ("Trans. (Alpha)", 2D) = "white" {}
    }
 
    Category {
        ZWrite Off
        Alphatest Greater 0
        Tags {Queue=Transparent}
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        SubShader {
            Material {
                Diffuse [_Color]
                Ambient [_Color]
                Shininess [_Shininess]
                Specular [_SpecColor]
                Emission [_Emission]
            }
            Pass {
                Lighting Off
                SeparateSpecular On
                SetTexture [_MainTex] {
                    Combine texture * primary, texture
                }
                SetTexture [_AlphaTex] {
                    constantColor [_Color]
                    Combine previous, texture * constant
                }
            }
        }
    }
}
 