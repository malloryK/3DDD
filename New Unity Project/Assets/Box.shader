Shader "Box" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "White"{}
	}
	SubShader {
		Pass{
			Material {
				Diffuse (1,1,1,1)
			}
			Lighting On
			Cull Front
			
		}
	
	}
}
