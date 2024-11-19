using UnityEngine;
using System.Collections;
using Lucky.Shaders.PostProcess;

namespace Lucky.Shaders.PostProcess
{
    public class EdgeDetectionWithNormalAndDepth : PostEffectsBase
    {
        [Range(0.0f, 1.0f)] public float edgesOnly = 0.0f;

        public Color edgeColor = Color.black;

        public Color backgroundColor = Color.white;

        public float sampleDistance = 1.0f;

        public float sensitivityDepth = 1.0f;

        public float sensitivityNormals = 1.0f;

        void OnEnable()
        {
            GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (MainMaterial != null)
            {
                MainMaterial.SetFloat("_EdgeOnly", edgesOnly);
                MainMaterial.SetColor("_EdgeColor", edgeColor);
                MainMaterial.SetColor("_BackgroundColor", backgroundColor);
                MainMaterial.SetFloat("_SampleDistance", sampleDistance);
                MainMaterial.SetVector("_Sensitivity", new Vector4(sensitivityNormals, sensitivityDepth, 0.0f, 0.0f));

                Graphics.Blit(src, dest, MainMaterial);
            }
            else
            {
                Graphics.Blit(src, dest);
            }
        }
    }
}