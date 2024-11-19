using System;
using Lucky.Framework;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using UnityEngine;
using static Lucky.Kits.Utilities.MathUtils;

namespace Lucky.Kits.Particle
{
    public class Particle : ManagedBehaviour
    {
        protected virtual void Kill()
        {
            ParticleSystem.Instance.ReleaseParticle(this);
            gameObject.SetActive(false);
        }

        public SpriteRenderer sr;
        public ManagedBehaviour track;
        public ParticleData data;
        public Sprite source;
        public Color color;
        public Color startColor;
        public Vector2 position;
        public Vector2 speed;
        public float size;
        public float startSize;
        public float life; // 随时间变化, <0 粒子死亡
        public float startLife; // 定值, 表示存活时间
        public float rotation;
        public float spin;
        public float spinFriction;

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            float delta = Timer.FixedDeltaTime(data.realtime);

            // 生命随时间流逝
            life -= delta;
            if (life <= 0f)
            {
                Kill();
                return;
            }

            // 面朝速度方向
            if (data.rotationType == ParticleData.RotationType.SameAsSpeedDirection)
            {
                if (speed != Vector2.zero)
                    rotation = speed.Radians();
            }
            else // 旋转
            {
                rotation += spin * delta;
                spin = Approach(spin, 0, spinFriction * delta);
            }

            // [0, 1]
            float left = life / startLife;
            float alpha = data.fadeType switch
            {
                ParticleData.FadeType.Linear => left,
                ParticleData.FadeType.Late => Min(1f, left / 0.25f),
                ParticleData.FadeType.InAndOut => left switch
                {
                    > 0.75f => 1f - (left - 0.75f) / 0.25f, // linear
                    < 0.25f => left / 0.25f, // linear
                    _ => 1f // static
                },
                _ => 1f
            };

            color = data.colorType switch
            {
                ParticleData.ColorType.Static => startColor,
                ParticleData.ColorType.Lerp => Color.Lerp(data.color2, startColor, left),
                ParticleData.ColorType.Blink => (Timer.BetweenInterval(life, 0.1f) ? startColor : data.color2),
                ParticleData.ColorType.Choose => startColor,
                _ => color
            };

            color = color.WithA(alpha);

            position += speed * delta;
            speed += data.acceleration * delta;
            speed = Approach(speed, Vector2.zero, data.friction * delta);
            speed *= Pow(data.speedMultiplier, delta);

            if (data.scaleOut)
            {
                size = startSize * Ease.CubicEaseInOut(left);
            }
        }

        public override void Render()
        {
            base.Render();
            Vector2 pos = position;
            if (track != null)
                pos += (Vector2)track.transform.position;

            sr.sprite = source;
            sr.color = color;
            transform.position = pos;
            transform.eulerAngles = new Vector3(0, 0, RadiansToDegree(rotation));
            transform.localScale = Vector3.one * size;
        }

        public virtual Particle ResetData()
        {
            transform.position = Vector3.one * int.MaxValue;
            gameObject.SetActive(true);
            return this;
        }

    }
}