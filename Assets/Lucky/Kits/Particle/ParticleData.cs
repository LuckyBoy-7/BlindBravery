using System;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lucky.Kits.Particle
{
    [CreateAssetMenu(fileName = "ParticleData")]
    public class ParticleData : ScriptableObject
    {

        #region Life

        [Header("Life")] public float lifeMin;
        public float lifeMax;
        public bool realtime;

        #endregion

        #region Texture

        [Header("Texture")] public List<Sprite> sprites = new(); // 源图

        #endregion

        #region Color

        public enum ColorType
        {
            Static, // 不做处理, 跟None差不多
            Choose, // 颜色从Color和Color2里面抽一个
            Blink, // 颜色在Color1和Color2之间交替
            Lerp // 颜色从1 lerp 到 2
        }

        public enum FadeType
        {
            None, // 不渐变
            Linear, // 线性的
            Late, // 也是线性的, 就是更晚开始淡出
            InAndOut // in-out缓入缓出的
        }

        [Header("Color")] public Color color; // 颜色1, 理论上没什么特殊情况的时候例子用的就是这个颜色
        public Color color2; // 颜色2
        public ColorType colorType; // 颜色模式
        public FadeType fadeType; // 淡入淡出模式

        #endregion

        #region Move

        [Header("Move")] public float speedMin; // 最小速度
        public float speedMax; // 最大速度
        [Tooltip("增加加速度的速率")] public float speedMultiplier; // 速度倍数
        public Vector2 acceleration; // 加速度
        public float friction; // 摩擦力
        public float direction; // 方向
        public float directionRange; // 方向转动最大角度

        #endregion

        #region Scale

        [Header("Scale")] public float size; // 大小
        public float sizeRange; // 大小变化范围
        public bool scaleOut; // 死亡的时候刚好缩放为0, 即Scale "Out"

        #endregion

        #region Rotation

        public enum RotationType
        {
            Identity, // 一开始弧度归一化
            Random, // 一开始随机弧度
            SameAsSpeedDirection // 面朝速度方向
        }

        [Header("Rotate")] public RotationType rotationType; // 旋转模式
        public float spinMin; // 旋转最小速度
        public float spinMax; // 旋转最大速度
        public float spinFriction;
        public bool randomFlipSpinDir; // 是否翻转旋转方向

        #endregion

        public ParticleData()
        {
            color = (color2 = Color.white);
            colorType = ColorType.Static;
            fadeType = FadeType.None;
            speedMin = (speedMax = 0f);
            speedMultiplier = 1f;
            acceleration = Vector2.zero;
            friction = 0f;
            direction = (directionRange = 0f);
            lifeMin = (lifeMax = 0f);
            size = 2f;
            sizeRange = 0f;
            spinMin = (spinMax = 0f);
            randomFlipSpinDir = false;
            rotationType = RotationType.Identity;
        }

        public ParticleData(ParticleData copyFrom)
        {
            sprites = copyFrom.sprites;
            color = copyFrom.color;
            color2 = copyFrom.color2;
            colorType = copyFrom.colorType;
            fadeType = copyFrom.fadeType;
            speedMin = copyFrom.speedMin;
            speedMax = copyFrom.speedMax;
            speedMultiplier = copyFrom.speedMultiplier;
            acceleration = copyFrom.acceleration;
            friction = copyFrom.friction;
            direction = copyFrom.direction;
            directionRange = copyFrom.directionRange;
            lifeMin = copyFrom.lifeMin;
            lifeMax = copyFrom.lifeMax;
            size = copyFrom.size;
            sizeRange = copyFrom.sizeRange;
            rotationType = copyFrom.rotationType;
            spinMin = copyFrom.spinMin;
            spinMax = copyFrom.spinMax;
            spinFriction = copyFrom.spinFriction;
            randomFlipSpinDir = copyFrom.randomFlipSpinDir;
            scaleOut = copyFrom.scaleOut;
            realtime = copyFrom.realtime;
        }

        public Particle Apply(Particle particle, Vector2 position)
        {
            return Apply(particle, position, direction);
        }

        public Particle Apply(Particle particle, Vector2 position, Color color)
        {
            return Apply(particle, null, position, direction, color);
        }

        public Particle Apply(Particle particle, Vector2 position, float direction)
        {
            return Apply(particle, null, position, direction, color);
        }

        public Particle Apply(Particle particle, Vector2 position, Color color, float direction)
        {
            return Apply(particle, null, position, direction, color);
        }

        public Particle Apply(Particle particle, ManagedBehaviour entity, Vector2 position, float direction, Color color)
        {
            particle.track = entity; // particle track的entity, 用来定位位置
            particle.data = this; // 给particle提供一个自身的引用
            particle.position = position; // 相对位置, 没有entitiy就是绝对位置
            // 有的话抽一个texture
            if (sprites.Count > 0)
            {
                particle.source = sprites.Choice();
            }
            // 都没有就用Draw的fallback
            else
            {
                if (ParticleSystem.ParticleSprite == null)
                    Debug.LogWarning("Draw.Particle is None");
                particle.source = ParticleSystem.ParticleSprite;
            }


            // 有range的话就roll一个
            if (sizeRange != 0f)
                particle.startSize = particle.size = size - sizeRange * 0.5f + RandomUtils.NextFloat(sizeRange);
            else
                particle.startSize = particle.size = size;

            // color
            if (colorType == ColorType.Choose)
                particle.startColor = particle.color = RandomUtils.Choose(color, color2);
            else
                particle.startColor = particle.color = color;

            float radians = direction - directionRange * 0.5f + RandomUtils.NextFloat() * directionRange;
            particle.speed = MathUtils.RadiansToVector(radians, RandomUtils.Range(speedMin, speedMax));
            particle.startLife = particle.life = RandomUtils.Range(lifeMin, lifeMax);

            // 自转
            particle.rotation = rotationType switch
            {
                RotationType.Random => RandomUtils.NextRadians(),
                RotationType.SameAsSpeedDirection => radians,
                _ => 0f
            };

            particle.spin = RandomUtils.Range(spinMin, spinMax);
            if (randomFlipSpinDir)
                particle.spin *= RandomUtils.Choose(1, -1);
            particle.spinFriction = spinFriction;

            return particle;
        }
    }
}