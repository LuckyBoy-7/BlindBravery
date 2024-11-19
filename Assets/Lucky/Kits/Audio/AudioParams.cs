using System;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AudioParams
{
    [Serializable]
    public class Pitch
    {
        public enum Variation
        {
            Small,
            Medium,
            Large,
            VerySmall,
        }

        /// 音高，相当于频率, pitch越高播放的时候音越尖, 时间越短
        public float Value;

        /// 直接设置
        public Pitch(float pitch)
        {
            Value = pitch;
        }

        /// 两数之间抽一个
        public Pitch(float minPitch, float maxPitch)
        {
            Value = Random.Range(minPitch, maxPitch);
        }

        /// 选一个预定义的enum
        public Pitch(Variation randomVariation)
        {
            switch (randomVariation)
            {
                case Variation.VerySmall:
                    Value = Random.Range(0.95f, 1.05f);
                    break;
                case Variation.Small:
                    Value = Random.Range(0.9f, 1.1f);
                    break;
                case Variation.Medium:
                    Value = Random.Range(0.75f, 1.25f);
                    break;
                case Variation.Large:
                    Value = Random.Range(0.5f, 1.5f);
                    break;
            }
        }
    }

    [Serializable]
    public class Repetition
    {
        public float MaxRepetitionFrequency;
        public string EntryId;

        public Repetition(float maxRepetitionFrequency, string entryId = "")
        {
            MaxRepetitionFrequency = maxRepetitionFrequency;
            EntryId = entryId;
        }
    }


    /// <summary>
    /// 只要实例化了就代表要使用random的音频, IsNoRepeating表示前后能不能roll到一样的值
    /// </summary>
    [Serializable]
    public class Randomization
    {
        public bool IsNoRepeating;

        public Randomization(bool isNoRepeating = true)
        {
            IsNoRepeating = isNoRepeating;
        }
    }

    [Serializable]
    public class Distortion
    {
        /// 沉闷的 
        public bool IsMuffled;

        public Distortion(bool isMuffled = true)
        {
            IsMuffled = isMuffled;
        }
    }
}