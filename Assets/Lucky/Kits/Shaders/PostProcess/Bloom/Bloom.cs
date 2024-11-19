using UnityEngine;
using System.Collections;

namespace Lucky.Shaders.PostProcess
{
	public class Bloom : PostEffectsBase
	{
		// Blur iterations - larger number means more blur.
		[Range(0, 4)] public int iterations = 3;

		// Blur spread for each iteration - larger value means more blur
		[Range(0.2f, 3.0f)] public float blurSpread = 0.6f;

		[Range(1, 8)] public int downSample = 2;

		[Range(-1f, 1.0f)] public float luminanceThreshold = 0.6f;

		// void OnRenderImage (RenderTexture src, RenderTexture dest) {
		// 	if (MainMaterial != null) {
		// 		MainMaterial.SetFloat("_LuminanceThreshold", luminanceThreshold);
		//
		// 		int rtW = src.width/downSample;
		// 		int rtH = src.height/downSample;
		// 		
		// 		RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
		// 		buffer0.filterMode = FilterMode.Bilinear;
		// 		
		// 		Graphics.Blit(src, buffer0, MainMaterial, 0);
		// 		
		// 		for (int i = 0; i < iterations; i++) {
		// 			MainMaterial.SetFloat("_BlurSize", 1.0f + i * blurSpread);
		// 			
		// 			RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
		// 			
		// 			// Render the vertical pass
		// 			Graphics.Blit(buffer0, buffer1, MainMaterial, 1);
		// 			
		// 			RenderTexture.ReleaseTemporary(buffer0);
		// 			buffer0 = buffer1;
		// 			buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
		// 			
		// 			// Render the horizontal pass
		// 			Graphics.Blit(buffer0, buffer1, MainMaterial, 2);
		// 			
		// 			RenderTexture.ReleaseTemporary(buffer0);
		// 			buffer0 = buffer1;
		// 		}
		//
		// 		MainMaterial.SetTexture ("_Bloom", buffer0);  
		// 		Graphics.Blit (src, dest, MainMaterial, 3);  
		//
		// 		RenderTexture.ReleaseTemporary(buffer0);
		// 	} else {
		// 		Graphics.Blit(src, dest);
		// 	}
		// }
		void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (MainMaterial != null)
			{
				MainMaterial.SetFloat("_LuminanceThreshold", luminanceThreshold);

				int rtW = src.width / downSample;
				int rtH = src.height / downSample;

				RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
				buffer0.filterMode = FilterMode.Bilinear;
				RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
				buffer1.filterMode = FilterMode.Bilinear;

				// 先提取亮的地方
				Graphics.Blit(src, buffer0, MainMaterial, 0);
				// 高斯模糊亮的地方
				for (int i = 0; i < iterations; i++)
				{
					MainMaterial.SetFloat("_BlurSize", 1.0f + i * blurSpread);

					Graphics.Blit(buffer0, buffer1, MainMaterial, 1);
					Graphics.Blit(buffer1, buffer0, MainMaterial, 2);
				}

				// 处理过的高斯模糊光照纹理和原图像混合
				MainMaterial.SetTexture("_Bloom", buffer0);
				Graphics.Blit(src, dest, MainMaterial, 3);

				RenderTexture.ReleaseTemporary(buffer0);
				RenderTexture.ReleaseTemporary(buffer1);
			}
			else
			{
				Graphics.Blit(src, dest);
			}
		}
	}
}