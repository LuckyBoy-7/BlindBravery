/*
我突然意识到想根据unity的cc框架写个ec框架似乎是不可能的, 我真的没办法干预unity的生命周期啊, 但我又想像蔚蓝那样做到精确完美
自己强行弄个Engine和添加LuckyComponent之类的只让我感到臃肿和难受(虽然功能勉强实现了), 而且到了后面肯定越来越难维护

也是勉强在Input里让FixedUpdate也能精确获取输入了, 感觉以后直接不写Update, 全写FixedUpdate里好了
但我又希望有时能在Update下调用, 因为这样更流畅而且误差小啊, 但fixedUpdate更稳定, 我什么都想要啊, 但用unity这样写很多东西都得绕弯, 但自己从零开始搭又很蠢
 */

using System;
using Lucky.Kits.Extensions;
using Lucky.Kits.Particle;
using Lucky.Framework.Inputs_;
using Lucky.Kits.Interactive;
using Lucky.Kits.Managers;
using Lucky.Kits.Managers.ObjectPool_;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Input = UnityEngine.Input;
using ParticleSystem = Lucky.Kits.Particle.ParticleSystem;

namespace Lucky.Framework
{
    /// <summary>
    /// 在设置界面保证Engine最先调用, 然后Engine去初始化各种Manager, 以保证更新顺序正确
    /// </summary>
    public class Engine : MonoBehaviour
    {
        public const float OneFrameTime = 1 / 60f;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Time.fixedDeltaTime = OneFrameTime;
            Settings.Initialize();
            Inputs.Initialize();
            ParticleSystem.Initialize();
            this.AddComponent<ObjectPoolManager>();
            this.AddComponent<GameCursor>();
            this.AddComponent<EventManager>();
            this.AddComponent<ParticleSystem>();
        }

        protected virtual void Update()
        {
            // Input有最高优先级
            Inputs.Update();
        }

        protected void FixedUpdate()
        {

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Q))
                    Time.timeScale /= 2;
                else if (Input.GetKeyDown(KeyCode.E))
                    Time.timeScale *= 2;
            }
#endif
        }
    }
}