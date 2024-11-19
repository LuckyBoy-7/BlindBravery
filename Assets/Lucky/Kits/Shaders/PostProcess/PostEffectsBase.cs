using System;
using UnityEngine;
using System.Collections;


namespace Lucky.Shaders.PostProcess
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class PostEffectsBase : MonoBehaviour
    {

        public Shader mainShader;
        private Material mainMaterial;

        public Material MainMaterial
        {
            get
            {
                mainMaterial = CheckShaderAndCreateMaterial(mainShader, mainMaterial);
                return mainMaterial;
            }
        }

        // Called when the platform doesn't support this effect
        protected void NotSupported()
        {
            enabled = false;
        }

        // Called when need to create the material used by this effect
        protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
        {
            if (shader == null)
            {
                return null;
            }

            if (shader.isSupported && material && material.shader == shader)
                return material;

            if (!shader.isSupported)
            {
                return null;
            }

            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            return null;
        }
    }
}