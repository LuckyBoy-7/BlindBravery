using System;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Collections;
using Lucky.Kits.Extensions;
using Lucky.Kits.Managers;
using Lucky.Kits.Utilities;
using UnityEngine;

namespace Lucky.Kits.Particle
{
    /// <summary>
    /// 对particle进行统一管理, 操控他们的create update render
    /// </summary>
    public class ParticleSystem : Singleton<ParticleSystem>
    {
        public static Sprite ParticleSprite;
        private DefaultDict<Type, List<Particle>> particles = new(() => new());
        private static Dictionary<string, ParticleData> particleDatas = new();

        public ParticleData this[string name] => particleDatas[name];

        public static void Initialize()
        {
            ParticleSprite = Resources.Load<Sprite>("Art/Primatives/Pixels/Particle");
            foreach (var particleData in Resources.LoadAll<ParticleData>("Data/Particle"))
            {
                particleDatas[particleData.name] = particleData;
            }
        }

        public void ReleaseParticle<T>(T particle) where T : Particle
        {
            // 如果使用gettype拿到的是子类的类型, 如果直接typeof(particle), 那么在不指定T, c#自动识别的情况下会当作Particle
            particles[particle.GetType()].Add(particle);
        }

        public void CheckAndMakeSure<T>() where T : Particle
        {
            if (particles[typeof(T)].Count == 0)
            {
                T particle = Instantiate(Resources.Load<T>($"Prefabs/Particles/{typeof(T).Name}"));
                particles[typeof(T)].Add(particle);
            }
        }

        /// <summary>
        /// 把type的数据传到particle里
        /// </summary>
        public void Emit<T>(string dataName, Vector2 position) where T : Particle
        {
            ParticleData data = particleDatas[dataName];
            CheckAndMakeSure<T>();
            data.Apply(particles[typeof(T)].Pop().ResetData(), position);
        }

        public void Emit<T>(string dataName, Vector2 position, float direction) where T : Particle
        {
            ParticleData data = particleDatas[dataName];
            CheckAndMakeSure<T>();
            data.Apply(particles[typeof(T)].Pop().ResetData(), position, direction);
        }

        public void Emit<T>(string dataName, Vector2 position, Color color) where T : Particle
        {
            ParticleData data = particleDatas[dataName];
            CheckAndMakeSure<T>();
            data.Apply(particles[typeof(T)].Pop().ResetData(), position, color);
        }

        public void Emit<T>(string dataName, Vector2 position, Color color, float direction) where T : Particle
        {
            ParticleData data = particleDatas[dataName];
            CheckAndMakeSure<T>();
            data.Apply(particles[typeof(T)].Pop().ResetData(), position, color, direction);
        }

        public void Emit<T>(string dataName, int amount, Vector2 position, Vector2 positionRange) where T : Particle
        {
            for (int i = 0; i < amount; i++)
            {
                Emit<T>(dataName, RandomUtils.Range(position - positionRange, position + positionRange));
            }
        }

        public void Emit<T>(string dataName, int amount, Vector2 position, Vector2 positionRange, float direction) where T : Particle
        {
            for (int i = 0; i < amount; i++)
            {
                Emit<T>(dataName, RandomUtils.Range(position - positionRange, position + positionRange), direction);
            }
        }

        public void Emit<T>(string dataName, int amount, Vector2 position, Vector2 positionRange, Color color) where T : Particle
        {
            for (int i = 0; i < amount; i++)
            {
                Emit<T>(dataName, RandomUtils.Range(position - positionRange, position + positionRange), color);
            }
        }

        public void Emit<T>(string dataName, int amount, Vector2 position, Vector2 positionRange, Color color, float direction) where T : Particle
        {
            for (int i = 0; i < amount; i++)
            {
                Emit<T>(dataName, RandomUtils.Range(position - positionRange, position + positionRange), color, direction);
            }
        }

        public void Emit<T>(string dataName, ManagedBehaviour track, int amount, Vector2 position, Vector2 positionRange) where T : Particle
        {
            ParticleData data = particleDatas[dataName];
            for (int i = 0; i < amount; i++)
            {
                CheckAndMakeSure<T>();
                data.Apply(
                    particles[typeof(T)].Pop().ResetData(), track, RandomUtils.Range(position - positionRange, position + positionRange), data.direction,
                    data.color
                );
            }
        }

        public void Emit<T>(string dataName, ManagedBehaviour track, int amount, Vector2 position, Vector2 positionRange, float direction) where T : Particle
        {
            ParticleData data = particleDatas[dataName];
            for (int i = 0; i < amount; i++)
            {
                CheckAndMakeSure<T>();
                data.Apply(
                    particles[typeof(T)].Pop().ResetData(), track, RandomUtils.Range(position - positionRange, position + positionRange), direction, data.color
                );
            }
        }
    }
}