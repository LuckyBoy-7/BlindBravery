using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Lucky.Framework;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lucky.Kits.Animation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Animator : ManagedBehaviour
    {
        public string StartId;
        public float Speed = 1f; // 动画播放速度倍率(负数意味着倒放)
        public bool UseRawDeltaTime; // 是否使用现实时间流速
        public Action<string> OnFinish; //  当前动画播放完切换到其他动画时干的事
        public Action<string> OnLoop; // 动画开始循环时干的事, 参数为切换动画后的动画id
        public Action<string> OnFrameChange; // 动画播放下一帧时干的事, 参数为当前动画id
        public Action<string> OnLastFrame; // 播放完动画最后一帧时干的事, 参数为当前动画id
        public Action<string, string> OnChange; // 动画切换时干的事, 参数为改变前的动画id和改变后的动画id
        private Sprite sprite;

        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>(StringComparer.OrdinalIgnoreCase); // 动画集合 id -> Animation
        private Animation currentAnimation; //  当前动画
        private float animationTimer; // 用来给Delay计时的

        private const string EmptyId = "ofjaispofdj;"; // 随机值

        [HideInInspector] public SpriteRenderer Sr;

        public List<Animation_SO> data;

        public class Animation
        {
            public float Delay; // 一帧动画持续多少秒
            public Sprite[] Frames; // 动画的图片帧序列
            public Chooser<string> Goto; // 播放完当前动画后应该跳到哪个动画
        }

        private void Awake()
        {
            Sr = GetComponent<SpriteRenderer>();

            AddEmpty(EmptyId);
            foreach (var so in data)
            {
                string id = so.Id;
                bool loop = so.Loop;
                float delay = so.Delay;
                Sprite[] sprites = so.Frames;
                string chooser = so.Goto;
                if (loop)
                    AddLoop(id, delay, sprites);
                else
                    Add(id, delay, chooser, sprites);
            }

            if (StartId != "")
                Play(StartId);
        }



        public override void Render()
        {
            base.Render();
            if (Sr && Sr.sprite != sprite)
                Sr.sprite = sprite;
        }

        /// <summary>
        /// 重置所有参数, 设置新的图片路径
        /// </summary>
        public void Reset()
        {
            animations = new Dictionary<string, Animation>(StringComparer.OrdinalIgnoreCase);
            currentAnimation = null;
            CurrentAnimationID = "";
            OnFinish = null;
            OnLoop = null;
            OnFrameChange = null;
            OnChange = null;
            CanAnimate = false;
        }

        /// <summary>
        /// 拿到对应id动画的某一帧图片
        /// </summary>
        public Sprite GetFrame(string animation, int frame) => animations[animation].Frames[frame];

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            if (!CanAnimate) // 不能animate直接退出
                return;

            animationTimer += Timer.FixedDeltaTime(UseRawDeltaTime) * Speed;
            if (Math.Abs(animationTimer) < currentAnimation.Delay) // 这一帧的时间还没到直接退出
                return;
            // 开始进入下一帧
            // 当前动画帧步进一
            CurrentAnimationFrame += Math.Sign(animationTimer);
            // "清空"timer
            animationTimer -= Math.Sign(animationTimer) * currentAnimation.Delay;

            // 如果当前帧在范围内, 则直接设置就好了
            if (CurrentAnimationFrame >= 0 && CurrentAnimationFrame < currentAnimation.Frames.Length)
            {
                SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
                return;
            }

            // 因为执行OnLastFrame后可能会导致动画状态改变, 所以先存一下
            string currentAnimationID = CurrentAnimationID;
            OnLastFrame?.Invoke(CurrentAnimationID);
            if (currentAnimationID != CurrentAnimationID)
                return;

            if (currentAnimation.Goto != null)
            {
                // id跳到下一个动画
                CurrentAnimationID = currentAnimation.Goto.Choose();
                OnChange?.Invoke(LastAnimationID, CurrentAnimationID);

                LastAnimationID = CurrentAnimationID;
                currentAnimation = animations[LastAnimationID];
                // 这里animation id已经变了, 所以用mod可能出问题
                if (CurrentAnimationFrame < 0)
                    CurrentAnimationFrame = CurrentAnimationTotalFrames - 1;
                else
                    CurrentAnimationFrame = 0;

                SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
                OnLoop?.Invoke(CurrentAnimationID);
            }
            else
            {
                if (CurrentAnimationFrame < 0)
                    CurrentAnimationFrame = 0;
                else
                    CurrentAnimationFrame = currentAnimation.Frames.Length - 1;

                // 这一轮动画结束, 因为状态没有切出去
                CanAnimate = false;
                string currentAnimationID2 = CurrentAnimationID;
                CurrentAnimationID = "";
                currentAnimation = null;
                animationTimer = 0f;
                OnFinish?.Invoke(currentAnimationID2);
            }
        }

        /// <summary>
        /// 设置当前帧图片, 顺便调整一下锚点
        /// </summary>
        private void SetFrame(Sprite sprite)
        {
            this.sprite = sprite;
            OnFrameChange?.Invoke(CurrentAnimationID);
        }

        /// <summary>
        /// 重置animationTimer并把当前帧设置为动画的第frame帧
        /// </summary>
        public void SetAnimationFrame(int frame)
        {
            animationTimer = 0f;
            CurrentAnimationFrame = frame % currentAnimation.Frames.Length;
            SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
        }

        /// <summary>
        /// 取特定图片的AddLoop
        /// </summary>
        public void AddLoop(string id, float delay, params Sprite[] frames)
        {
            animations[id] = new Animation
            {
                Delay = delay,
                Frames = frames,
                Goto = new Chooser<string>(id, 1f)
            };
        }


        private void AddEmpty(string id)
        {
            animations[id] = new Animation
            {
                Delay = 0f,
                Frames = new Sprite[] { null },
                Goto = null
            };
        }

        public void Add(string id, float delay, string into, params Sprite[] frames)
        {
            animations[id] = new Animation
            {
                Delay = delay,
                Frames = frames,
                Goto = Chooser<string>.FromString<string>(into)
            };
        }

        /// <summary>
        /// 清空动画
        /// </summary>
        public void ClearAnimations() => animations.Clear();

        /// <summary>
        /// 清空动画并进入待机状态
        /// </summary>
        public void PlayEmpty()
        {
            Play(EmptyId);
        }

        /// <summary>
        /// 播放对应id的动画
        /// </summary>
        /// <param name="id">动画的id</param>
        /// <param name="restart">播放相同id动画是否restart</param>
        /// <param name="randomizeFrame">是否随机化animationTimer和CurrentAnimationFrame</param>
        public void Play(string id, bool restart = false, bool randomizeFrame = false)
        {
            if (CurrentAnimationID != id || restart)
            {
                OnChange?.Invoke(LastAnimationID, id);

                CurrentAnimationID = id;
                LastAnimationID = id;
                currentAnimation = animations[id];
                CanAnimate = currentAnimation.Delay > 0f;
                if (randomizeFrame)
                {
                    animationTimer = RandomUtils.NextFloat(currentAnimation.Delay);
                    CurrentAnimationFrame = RandomUtils.Range(currentAnimation.Frames.Length);
                }
                else
                {
                    animationTimer = 0f;
                    CurrentAnimationFrame = 0;
                }

                // debug
                if (currentAnimation.Frames.Length == 0)
                {
                    Debug.LogWarning($"animation of [{CurrentAnimationID}] has 0 frames");
                }

                SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
            }
        }

        /// <summary>
        /// 播放对应id的动画并快进offset时间
        /// </summary>
        public void PlayOffset(string id, float offset, bool restart = false)
        {
            if (CurrentAnimationID != id || restart)
            {
                OnChange?.Invoke(LastAnimationID, id);

                CurrentAnimationID = id;
                LastAnimationID = id;
                currentAnimation = animations[id];
                if (currentAnimation.Delay > 0f)
                {
                    CanAnimate = true;
                    float num = currentAnimation.Delay * currentAnimation.Frames.Length * offset;
                    CurrentAnimationFrame = 0;
                    while (num >= currentAnimation.Delay)
                    {
                        int currentAnimationFrame = CurrentAnimationFrame;
                        CurrentAnimationFrame = currentAnimationFrame + 1;
                        num -= currentAnimation.Delay;
                    }

                    CurrentAnimationFrame %= currentAnimation.Frames.Length;
                    animationTimer = num;
                    SetFrame(currentAnimation.Frames[CurrentAnimationFrame]);
                    return;
                }

                animationTimer = 0f;
                CanAnimate = false;
                CurrentAnimationFrame = 0;
                SetFrame(currentAnimation.Frames[0]);
            }
        }

        /// <summary>
        /// 播放对应id的动画直到动画停止
        /// </summary>
        public IEnumerator PlayRoutine(string id, bool restart = false)
        {
            Play(id, restart);
            return PlayUtil();
        }

        /// <summary>
        /// 播放对应id的动画直到动画停止
        /// </summary>
        public IEnumerator ReverseRoutine(string id, bool restart = false)
        {
            PlayReverse(id, restart);
            return PlayUtil();
        }

        /// <summary>
        /// 直到当前动画停止
        /// </summary>
        private IEnumerator PlayUtil()
        {
            while (CanAnimate)
            {
                yield return null;
            }
        }

        /// <summary>
        /// 将动画播放方向设置为逆序播放
        /// </summary>
        /// <param name="id"></param>
        /// <param name="restart"></param>
        public void PlayReverse(string id, bool restart = false)
        {
            Play(id, restart);
            if (Speed > 0f)
                Speed *= -1f;
        }

        /// <summary>
        /// 查看是否注册过id为id的动画
        /// </summary>
        public bool Has(string id) => id != null && animations.ContainsKey(id);

        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            CanAnimate = false;
            currentAnimation = null;
            CurrentAnimationID = "";
        }

        /// <summary>
        /// 是否可以播放动画
        /// </summary>
        public bool CanAnimate { get; private set; }

        /// <summary>
        /// 当前动画id
        /// </summary>
        public string CurrentAnimationID { get; private set; }

        /// <summary>
        /// 上一个动画id
        /// </summary>
        public string LastAnimationID { get; private set; }

        /// <summary>
        /// 播放到当前动画的哪一帧了
        /// </summary>
        public int CurrentAnimationFrame { get; private set; }

        /// <summary>
        /// 当前动画总帧数
        /// </summary>
        public int CurrentAnimationTotalFrames => currentAnimation != null ? currentAnimation.Frames.Length : 0;
    }
}