using UnityEngine;
using System.Collections;
using Lucky.Shaders.PostProcess;

namespace Lucky.Shaders.PostProcess
{
    public class GaussianBlur : PostEffectsBase
    {
        // Blur iterations - larger number means more blur.高斯模糊的迭代次数
        [Range(0, 4)] public int iterations = 3;

        // Blur spread for each iteration - larger value means more blur，高斯模糊的迭代范围
        [Range(0.2f, 3.0f)] public float blurSpread = 0.6f;

        // blurSpread downSample 都是出于性能的考虑。在高斯核维数不变的情况下 BlurSize
        // 大，模糊程度越高 但采样数却不会受到影响。但过大的 BlurSize 值会造成虚影 这可能并不是
        // 我们希望的。而 downSample 越大 需要处理的像素数越少，同时也能进一步提高模糊程度
        // 	过大的 downSample 可能会使图像像素化。
        [Range(1, 8)] // 高斯模糊的缩放系数
        public int downSample = 2;

        // / 1st edition: just apply blur
        // void OnRenderImage(RenderTexture src, RenderTexture dest) {
        // 	if (MainMaterial != null) {
        // 		int rtW = src.width;
        // 		int rtH = src.height;
        // 		RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
        //
        // 		// Render the vertical pass
        // 		Graphics.Blit(src, buffer, MainMaterial, 0);
        // 		// Render the horizontal pass
        // 		Graphics.Blit(buffer, dest, MainMaterial, 1);
        //
        // 		RenderTexture.ReleaseTemporary(buffer);
        // 	} else {
        // 		Graphics.Blit(src, dest);
        // 	}
        // } 

        /// 2nd edition: scale the render texture
//	void OnRenderImage (RenderTexture src, RenderTexture dest) {
//		if (MainMaterial != null) {
//			int rtW = src.width/downSample;
//			int rtH = src.height/downSample;
//			RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
//			buffer.filterMode = FilterMode.Bilinear;
//
//			// Render the vertical pass
//			Graphics.Blit(src, buffer, MainMaterial, 0);
//			// Render the horizontal pass
//			Graphics.Blit(buffer, dest, MainMaterial, 1);
//
//			RenderTexture.ReleaseTemporary(buffer);
//		} else {
//			Graphics.Blit(src, dest);
//		}
//	}

        // /// 3rd edition: use iterations for larger blur
        // void OnRenderImage (RenderTexture src, RenderTexture dest) {
        // 	if (MainMaterial != null) {
        // 		int rtW = src.width/downSample;
        // 		int rtH = src.height/downSample;
        //
        // 		RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);  // 感觉有点像是把src根据bilinear缩放了
        // 		buffer0.filterMode = FilterMode.Bilinear;
        //
        // 		Graphics.Blit(src, buffer0);
        //
        //
        // 		for (int i = 0; i < iterations; i++) {
        // 			MainMaterial.SetFloat("_BlurSize", 1.0f + i * blurSpread);
        // 		
        // 			RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
        // 			// Render the vertical pass
        // 			Graphics.Blit(buffer0, buffer1, MainMaterial, 0);
        // 		
        // 			RenderTexture.ReleaseTemporary(buffer0);
        // 			buffer0 = buffer1;
        // 			buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
        // 		
        // 			// Render the horizontal pass
        // 			Graphics.Blit(buffer0, buffer1, MainMaterial, 1);
        // 		
        // 			RenderTexture.ReleaseTemporary(buffer0);
        // 			buffer0 = buffer1;
        // 		}
        // 		
        // 		Graphics.Blit(buffer0, dest);
        // 		RenderTexture.ReleaseTemporary(buffer0);
        // 	} else {
        // 		Graphics.Blit(src, dest);
        // 	}
        // }

        /// 4th 调整
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (MainMaterial != null)
            {
                int rtW = src.width / downSample;
                int rtH = src.height / downSample;

                RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0); // 感觉有点像是把src根据bilinear缩放了
                buffer0.filterMode = FilterMode.Bilinear;
                RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
                buffer1.filterMode = FilterMode.Bilinear;

                Graphics.Blit(src, buffer0);

                for (int i = 0; i < iterations; i++)
                {
                    MainMaterial.SetFloat("_BlurSize", 1.0f + i * blurSpread);
                    // right
                    Graphics.Blit(buffer0, buffer1, MainMaterial, 0);
                    Graphics.Blit(buffer1, buffer0, MainMaterial, 1);

                    // wrong
                    // Graphics.Blit(buffer0, buffer1, MainMaterial);
                    // Graphics.Blit(buffer0, buffer1, MainMaterial, -1);
                    // (buffer0, buffer1) = (buffer1, buffer0);
                }

                Graphics.Blit(buffer0, dest);
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