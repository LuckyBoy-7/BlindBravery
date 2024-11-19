using Lucky.Kits.Utilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Lucky.Kits.Animation
{
    [CreateAssetMenu(fileName = "Animation")]
    public class Animation_SO : ScriptableObject
    {
        public string Id;
        public float Delay; // 一帧动画持续多少秒

        public bool Loop;
        [PreviewField] public Sprite[] Frames; // 动画的图片帧序列
        [HideIf("Loop")] public string Goto; // 播放完当前动画后应该跳到哪个动画
    }
}