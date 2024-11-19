using System;
using Lucky.Shaders.PostProcess;
using UnityEngine;

namespace Lucky.Shaders
{

    [ExecuteInEditMode]
    public class Blur : PostEffectsBase
    {
        /// Blur iterations - larger number means more blur.
        public int iterations = 3;

        /// Blur spread for each iteration. Lower values
        /// give better looking blur, but require more iterations to
        /// get large blurs. Value is usually between 0.5 and 1.0.
        public float blurSpread = 0.6f;

        public float off;


        // --------------------------------------------------------
        // The blur iteration shader.
        // Basically it just takes 4 texture samples and averages them.
        // By applying it repeatedly and spreading out sample locations
        // we get a Gaussian blur approximation.


        // Performs one blur iteration.
        // 由于只做一次的话只有对角的alpha较大，blur出来就不是很圆，有点胖胖的菱形的感觉
        // 再做一次可以想象一下对角的alpha会向中间扩散，做的越多，blur的越圆（应该是吧，现在还不是很确定）
        // 1, 1.2, 35.3 可以试一下这个参数然后调整iteration感受一下
        public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
        {
            float off = 0.5f + iteration * blurSpread;
            // 采样的越多球越大
            Graphics.BlitMultiTap(
                source, dest, MainMaterial,
                new Vector2(-off, -off),
                new Vector2(-off, off),
                new Vector2(off, off),
                new Vector2(off, -off)
            );
        }

        // Downsamples the texture to a quarter resolution.
        private void DownSample4x(RenderTexture source, RenderTexture dest)
        {
            // 以下代码含义是针对Graphics.BlitMultiTap所传入的每一个偏移值，进行一次纹理采样（最多4次），并且与前一步的渲染结果进行叠加(不是在在前一张图上再做一次吧)
            // 经我研究应该是在src上做了四次然后叠在src上传到dest上，结果就是有5个球4个对角一个中心
            // 由于shader是四分之一混合的，意味着四个对焦的球alpha=0.25，交叉部分为0.5，这个可以从metaball shader的cutoff里验证
            Graphics.BlitMultiTap(
                source, dest, MainMaterial,
                new Vector2(-off, -off),
                new Vector2(-off, off),
                new Vector2(off, off),
                new Vector2(off, -off)
            );
        }

        // Called by the camera to apply the image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            int rtW = source.width / 4;
            int rtH = source.height / 4;
            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
            RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

            // 我的理解是
            // Copy source to the 4x4 smaller texture.
            // 这里应该是降采样提高性能吧
            DownSample4x(source, buffer0);

            // Blur the small texture
            for (int i = 0; i < iterations; i++)
            {
                FourTapCone(buffer0, buffer1, i);
                (buffer0, buffer1) = (buffer1, buffer0);
            }

            Graphics.Blit(buffer0, destination);

            RenderTexture.ReleaseTemporary(buffer0);
            RenderTexture.ReleaseTemporary(buffer1);
        }
    }
}