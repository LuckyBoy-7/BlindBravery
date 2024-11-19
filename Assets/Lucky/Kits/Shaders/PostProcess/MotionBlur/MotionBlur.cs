using UnityEngine;
using System.Collections;

namespace Lucky.Shaders.PostProcess
{
    public class MotionBlur : PostEffectsBase
    {
        [Range(0.0f, 1f)] public float blurAmount = 0.5f;

        private RenderTexture accumulationTexture;

        void OnDisable()
        {
            DestroyImmediate(accumulationTexture);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (MainMaterial == null)
            {
                Graphics.Blit(src, dest);
                return;
            }

            // Create the accumulation texture
            if (accumulationTexture == null || accumulationTexture.width != src.width ||
                accumulationTexture.height != src.height) // 应该是指中途src的宽高改变了吧
            {
                DestroyImmediate(accumulationTexture);
                accumulationTexture = new RenderTexture(src.width, src.height, 0) { hideFlags = HideFlags.HideAndDontSave };
                // Graphics.Blit(src, accumulationTexture);  // 保留原图
            }

            // We are accumulating motion over frames without clear/discard
            // by design, so silence any performance warnings from Unity
            // 别人的的理解是，正常的RenderTexture都应该在下一帧前销毁掉，所以若我们不在每次OnRenderTexture函数过后销毁脚本中的RenderTexture变量，Unity就会发出性能警告，所以MarkRestoreExpected函数就是为了告诉Unity，我确实需要这张RenderTexture保持它当前的样子而不是在渲染完每一帧后销毁掉他。
            // 问题是我没开它也妹警告啊(，所以我懵逼了很久
            // 我在想虽然accumulationTexture存了之前所有画面信息的总和，但占比会爆炸式减小，所以问题不大（我一开始还想着用queue来解决呢）
            // 这个方法已被弃用, 现在无效了
            // accumulationTexture.MarkRestoreExpected();

            MainMaterial.SetFloat("_BlurAmount", blurAmount);

            // 这样会不会好理解点
            // 就是Blit的dest实际上表示在颜色缓冲区里的信息, 然后从src往dist里画, 本质上就是在dist上对src做了一遍的后处理, 所以在pass blend可以混合src和accumulation
            Graphics.Blit(src, accumulationTexture, MainMaterial);
            Graphics.Blit(accumulationTexture, dest);
        }
    }
}