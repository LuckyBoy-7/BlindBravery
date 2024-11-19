using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucky.Kits.Collections;
using Lucky.Kits.Extensions;
using Lucky.Kits.Managers;
using Lucky.Kits.Utilities;
using UnityEngine;

namespace Lucky.Kits.Audio
{
    public class AudioController : Singleton<AudioController>
    {
        public AudioSource LoopSourcePrefab;
        [SerializeField] private List<AudioSource> loopSources = new();

        private Trie<AudioClip> sfxClips = new Trie<AudioClip>(true);
        private Trie<AudioClip> loopClips = new Trie<AudioClip>(true);

        private List<AudioSource> sourcePool = new();

        /// <summary>
        /// 在设置Pause状态的时候清理掉所有播放完的source
        /// 我说怎么pause 32个音效之后就没声了呢, 而且还是正正好好32, 原来在Audio设置里Max Real Voices = 32
        /// </summary>
        private List<AudioSource> ActiveSFXSources
        {
            get
            {
                List<AudioSource> nxtActiveSFX = new();
                foreach (var source in activeSFX)
                {
                    print(source.loop);
                    if (source.time == 0 && !source.loop)
                        sourcePool.Add(source); // 结束的放pool里
                    else
                        nxtActiveSFX.Add(source); // 没结束的继续存着
                }

                activeSFX = nxtActiveSFX;
                return activeSFX;
            }
        }

        private List<AudioSource> activeSFX = new List<AudioSource>();

        /// 表示最后一次 音频id + repetitionId 的播放时间戳 
        private DefaultDict<string, float> idToLastPlayedTimestamp = new DefaultDict<string, float>(() => float.NegativeInfinity);

        /// 表示最后一次播放的随机音频对应的#后边的id  
        private DefaultDict<string, string> lastPlayedSounds = new DefaultDict<string, string>(() => "");

        private const char SoundIdRepeatDelimiter = '#';
        private const float DefaultSpatialBlend = 0.75f;

        protected override void Awake()
        {
            base.Awake();
            // 最高层级
            transform.parent = null;

            // 加载资源
            foreach (var clip in Resources.LoadAll<AudioClip>("Audio/SFX"))
                sfxClips.Add(clip.name, clip, SoundIdRepeatDelimiter);

            foreach (var clip in Resources.LoadAll<AudioClip>("Audio/Loops"))
                loopClips.Add(clip.name, clip);
        }

        #region Sfx

        public AudioSource PlaySound2D(AudioClip clip)
        {
            AudioSource source = InstantiateAudioObject(clip, Vector3.zero, false);

            source.time = 0;
            source.spatialBlend = 0f; // 因为2D，所以声音不随距离衰减
            DontDestroyOnLoad(source.gameObject);
            activeSFX.Add(source);

            return source;
        }

        /// <summary>
        /// 播放2D音效
        /// </summary>
        /// <param name="soundId">音频名称</param>
        /// <param name="volume">音量</param>
        /// <param name="skipToTime">播放起点</param>
        /// <param name="pitch">音高</param>
        /// <param name="repetition">重复, 用来合并产生时间相近的音频</param>
        /// <param name="randomization">随机</param>
        /// <param name="distortion">扰动</param>
        /// <param name="looping">循环</param>
        /// <returns></returns>
        public AudioSource PlaySound2D(
            string soundId,
            float volume = 1f,
            float skipToTime = 0f,
            AudioParams.Pitch pitch = null,
            AudioParams.Repetition repetition = null,
            AudioParams.Randomization randomization = null,
            AudioParams.Distortion distortion = null,
            bool looping = false
        )
        {
            AudioSource source = PlaySound3D(soundId, Vector3.zero, volume, skipToTime, pitch, repetition, randomization, distortion, looping);

            if (source != null)
            {
                source.spatialBlend = 0f; // 因为2D，所以声音不随距离衰减
                DontDestroyOnLoad(source.gameObject);
            }

            return source;
        }

        public AudioSource PlaySound3D(
            string soundId,
            Vector3 position,
            float volume = 1f,
            float skipToTime = 0f,
            AudioParams.Pitch pitch = null,
            AudioParams.Repetition repetition = null,
            AudioParams.Randomization randomization = null,
            AudioParams.Distortion distortion = null,
            bool looping = false
        )
        {
            // 输入空字符串
            if (string.IsNullOrEmpty(soundId))
            {
                Debug.LogWarning("You are trying to play sound with empty id");
                return null;
            }

            // 如果设置了频率并且播放频率超过了限制
            if (repetition != null && IsRepetitionTooFrequent(soundId, repetition.MaxRepetitionFrequency, repetition.EntryId))
                return null;

            // roll一个id出来
            string randomVariationId = soundId;
            if (randomization != null)
                randomVariationId = GetRandomVariationOfSound(soundId, randomization.IsNoRepeating);

            // 尝试创建
            var source = CreateAudioSourceForSound(randomVariationId, position, looping);
            if (source == null)
            {
                Debug.LogWarning("No corresponding audioClip is found");
                return null;
            }

            source.volume = volume;
            source.time = source.clip.length * skipToTime;

            if (pitch != null)
                source.pitch = pitch.Value;

            if (distortion != null && distortion.IsMuffled)
                MuffleSource(source);

            activeSFX.Add(source);
            return source;
        }

        public void SetAllSfxPausedState(bool paused)
        {
            // print(ActiveSFXSources.Count);
            foreach (var source in ActiveSFXSources)
            {
                if (paused)
                    source.Pause();
                else
                    source.UnPause();
            }
        }

        public AudioClip GetAudioClip(string soundId)
        {
            List<AudioClip> clips = sfxClips.Get(soundId);
            return clips.Count == 0 ? null : clips[0];
        }

        private AudioSource CreateAudioSourceForSound(string soundId, Vector3 position, bool looping)
        {
            // 尝试通过id拿到clip
            AudioClip sound = GetAudioClip(soundId);

            if (sound != null)
                return InstantiateAudioObject(sound, position, looping);
            return null;
        }

        /// <summary>
        /// 生成一个挂载AudioSource的GameObject并初始化各参数
        /// </summary>
        private AudioSource InstantiateAudioObject(AudioClip clip, Vector3 pos, bool looping)
        {
            AudioSource source;
            string name = "Audio_" + clip.name;
            if (sourcePool.Count > 0)
            {
                source = sourcePool.Pop();
                source.gameObject.name = name;
            }
            else
            {
                GameObject go = new GameObject(name);
                source = go.AddComponent<AudioSource>();
            }

            source.transform.position = pos;
            source.clip = clip;
            source.loop = looping;
            source.spatialBlend = DefaultSpatialBlend;
            source.playOnAwake = false;

            source.Play();

            return source;
        }

        /// <summary>
        /// 音频的播放频率是否超过了所限制的频率
        /// </summary>
        /// <param name="soundId">音频id</param>
        /// <param name="frequencyMax">最大频率</param>
        /// <param name="entrySuffix">频率id</param>
        private bool IsRepetitionTooFrequent(string soundId, float frequencyMax, string entrySuffix = "")
        {
            float time = Timer.GetTime(true); // 现实时间戳
            string soundKey = soundId + entrySuffix;

            if (time - 1 / frequencyMax > idToLastPlayedTimestamp[soundKey])
            {
                idToLastPlayedTimestamp[soundKey] = time;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获得对应音频id的随机版本，比如从attack中得到attack#1, attack#3, attack#6
        /// </summary>
        /// <param name="soundPrefix">音频id</param>
        /// <param name="isNoRepeating">是否保证前后随机到不同的音频</param>
        /// <returns></returns>
        private string GetRandomVariationOfSound(string soundPrefix, bool isNoRepeating)
        {
            // 比如我们想播放attack
            // 他就会找到形如attack#1, attack#2, attack#3的clip，#后面必须填从1开始的数字
            List<AudioClip> variations = sfxClips.Get(soundPrefix);
            if (variations.Count == 0) // 没找到就结束
                return soundPrefix;
            if (variations.Count == 1 && isNoRepeating)
            {
                Debug.LogWarning("You're trying to play an audioClip without repetition, but there's only one clip found!");
                return soundPrefix;
            }

            // 抽一个
            int index = Random.Range(0, variations.Count);
            if (isNoRepeating)
            {
                while (lastPlayedSounds[soundPrefix] == variations[index].name)
                    index = Random.Range(0, variations.Count);
                lastPlayedSounds[soundPrefix] = variations[index].name;
            }

            return variations[index].name;
        }

        /// <summary>
        /// 增加低音的感觉, 滤掉频率高的波
        /// </summary>
        private void MuffleSource(AudioSource source, float cutoff = 300f)
        {
            var filter = source.gameObject.AddComponent<AudioLowPassFilter>();
            filter.cutoffFrequency = cutoff;
        }

        /// <summary>
        /// 尝试销毁AudioLowPassFilter
        /// </summary>
        private void UnMuffleSource(AudioSource source)
        {
            var lowPassFilter = source.GetComponent<AudioLowPassFilter>();
            if (lowPassFilter != null)
                Destroy(lowPassFilter);
        }

        #endregion

        #region Loop

        public AudioSource CreateLoopSourcePrefab() => Instantiate(LoopSourcePrefab, transform);

        public void CheckAndMakeSureLoopSource(int idx)
        {
            while (idx + 1 > loopSources.Count)
                loopSources.Add(CreateLoopSourcePrefab());
        }

        public void PlayLoop(AudioSource source, string name)
        {
            AudioClip loop = GetLoop(name);
            source.clip = loop;
            source.pitch = 1f;
            source.time = 0f;
            source.Play();
        }

        public void PlayLoop(string name, int idx)
        {
            TrySetLoop(name, idx);
            loopSources[idx].time = 0f;
            loopSources[idx].Play();
        }

        private void TrySetLoop(string loopName, int sourceIndex = 0)
        {
            CheckAndMakeSureLoopSource(sourceIndex);
            AudioClip loop = GetLoop(loopName);

            if (loop != null)
            {
                loopSources[sourceIndex].clip = loop;
                loopSources[sourceIndex].pitch = 1f;
            }
        }

        private AudioClip GetLoop(string loopName)
        {
            var loops = loopClips.Get(loopName);
            return loops.Count == 0 ? null : loops[0];
        }

        private IEnumerator DoFadeToVolume(float duration, float volume, AudioSource source)
        {
            float speed = (volume - source.volume) / duration;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            while (source.volume != volume)
            {
                source.volume = MathUtils.Approach(source.volume, volume, speed * Timer.FixedDeltaTime());
                yield return null;
            }
        }

        /// <param name="newLoop">fadeIn的loopId</param>
        /// <param name="volume">音量</param>
        /// <param name="duration">fade时长</param>
        /// <param name="sourceIndex">选择哪个audioSource播放</param>
        /// <returns></returns>
        public IEnumerator CrossFade(string newLoop, float volume, float duration, int sourceIndex = 0)
        {
            CheckAndMakeSureLoopSource(sourceIndex);
            AudioSource origSource = loopSources[sourceIndex];
            if (origSource.clip != null && origSource.isPlaying)
                StartCoroutine(DoFadeToVolume(duration, 0f, origSource)); // HACK: also fade out 2nd loop source here

            AudioSource source = CreateLoopSourcePrefab();
            loopSources[sourceIndex] = source;
            PlayLoop(source, newLoop);
            StartCoroutine(DoFadeToVolume(duration, volume, source));
            yield return duration;
            Destroy(origSource.gameObject, 1);
        }

        #endregion

    }
}