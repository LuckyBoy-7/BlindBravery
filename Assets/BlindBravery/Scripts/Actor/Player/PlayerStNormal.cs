using System;
using Lucky.Kits.Extensions;
using Lucky.Kits;
using Lucky.Framework.Inputs_;
using BlindBravery.Enums;
using Lucky.Kits.Utilities;
using UnityEngine;
using static Lucky.Kits.Utilities.MathUtils;

namespace BlindBravery.Actor.Player
{
    public partial class Player
    {

        // 最大下落速度(不按按键的时候)
        public const float MaxFallSpeedY = 160f; // 最大下落速度
        private const float MaxFastFallSpeedY = 240f;
        private const float FallAccel = 300f;

        // 重力
        private const float Gravity = 900f; // 重力加速度
        private const float HalfGravThreshold = 40f; // y方向速度在范围内时重力减半

        // move
        private const float MaxRunSpeed = 90; // 最大移动速度

        private const float RunAccel = 1000; // 移动时的加速度
        private const float RunReduce = 400; // 移动时的加速度
        private const float AirMult = 0.65f;

        // Jump
        private const float JumpBoostH = 40f; // 起跳时获得的额外水平速度
        private const float JumpSpeedV = 105f; // 起跳时跳跃y方向速度
        private const float JumpGraceTime = 0.1f; // 狼跳时间
        private const float VarJumpTime = 0.2f; // 跳跃初期上升时间
        private const int WallJumpCheckDistH = 3; // 踢墙跳

        // retention
        private const float WallSpeedRetentionTime = 0.06f;

        // slide
        private const float WallSlideTime = 1.2f;
        public const float WallSlideStartSpeedY = 20f; // 开始下滑时的最小速度


        private float varJumpSpeed;
        private float varJumpTimer;
        private float jumpGraceTimer;
        private float wallSpeedRetentionTimer;
        private float wallSpeedRetained;
        private float maxFall;
        private int wallSlideDir;
        private float wallSlideTimer = WallSlideTime;

        private int NormalUpdate()
        {
            // 在斜坡上不能抓(或许可能可以调宽松点)
            if (!nearSlope || !onGround)
            {
                // to climb, 必须按着抓, 速度不向上, 面朝方向和速度方向一样, 或者不动的时候
                if (Inputs.Grab.Check && rb.velocity.y <= 0 && Sign(rb.velocity.x) != -(int)facing && !IsTired)
                {
                    // 进入攀爬状态
                    if (CollideCheckBy((int)facing * ClimbCheckDistH * Vector2.right, true))
                    {
                        return StClimb;
                    }

                    // 向上吸附
                    if (Inputs.MoveY >= 0f)
                        for (int i = 2; i <= ClimbUpCheckDist; i++)
                        {
                            bool nearSolid = CollideCheckBy(Vector2.up * i + Vector2.right * (int)facing * ClimbCheckDistH, true);
                            bool nearSlope = CollideCheckSlopeBy(Vector2.up * i + Vector2.right * (int)facing * ClimbCheckDistH);
                            if (nearSolid && !nearSlope && !CollideCheckBy(Vector2.up * i))
                            {
                                MoveV(i);
                                return StClimb;
                            }
                        }
                }
            }

            if (!nearSlope || !onGround)
            {
                // 左右移动
                float accelX = Abs(rb.velocity.x) > MaxRunSpeed && Sign(rb.velocity.x) == moveX ? RunReduce : RunAccel;
                if (!onGround)
                    accelX *= AirMult;
                rb.SetSpeedX(Approach(rb.velocity.x, MaxRunSpeed * moveX, accelX * Timer.DeltaTime()));
            }
            else
            {
                // 左右移动
                float accelX = Abs(rb.velocity.x) > MaxRunSpeed && Sign(rb.velocity.x) == moveX ? RunReduce : RunAccel;
                if (!onGround)
                    accelX *= AirMult;
                // * 0.95是误差补充
                rb.SetSpeedX(Approach(rb.velocity.x, MaxRunSpeed * moveX * Cos(VectorToRadians(slopePerpendicularRight)), accelX * Timer.DeltaTime()));
                rb.SetSpeedY(Approach(rb.velocity.y, MaxRunSpeed * moveX * Sin(VectorToRadians(slopePerpendicularRight)), accelX * Timer.DeltaTime() * 0.55f));
            }

            // 坠落
            if (Inputs.MoveY.Value == -1f && -rb.velocity.y >= MaxFallSpeedY)
            {
                maxFall = Approach(maxFall, MaxFastFallSpeedY, FallAccel * Timer.DeltaTime());
                float mid = (MaxFallSpeedY + MaxFastFallSpeedY) / 2;
                if (-rb.velocity.y >= mid)
                {
                    float kCloseToFastFall = Min(1f, (-rb.velocity.y - mid) / (MaxFastFallSpeedY - mid));
                    sr.transform.SetScaleX(Lerp(1f, 0.5f, kCloseToFastFall));
                    sr.transform.SetScaleY(Lerp(1f, 1.5f, kCloseToFastFall));
                }
            }
            else
            {
                maxFall = Approach(maxFall, MaxFallSpeedY, FallAccel * Timer.DeltaTime());
            }

            if (!onGround)
            {
                float fallSpeed = maxFall;
                // 对应主动滑墙和没体力硬抓两种情况, 前提是不按着下
                if ((moveX == (int)facing || (moveX == 0 && Inputs.Grab.Check)) && Inputs.MoveY.Value != -1)
                {
                    if (rb.velocity.y <= 0f && wallSlideTimer > 0f && CollideCheckBy(Vector2.right * (int)facing))
                    {
                        wallSlideDir = (int)facing;
                        fallSpeed = Lerp(160f, WallSlideStartSpeedY, wallSlideTimer / WallSlideTime);
                    }
                }

                float g = Inputs.Jump.Check && Abs(rb.velocity.y) < HalfGravThreshold ? Gravity / 2 : Gravity;
                rb.SetSpeedY(Approach(rb.velocity.y, -fallSpeed, g * Timer.DeltaTime()));
            }

            // 跳的上升过程
            if (varJumpTimer > 0f)
            {
                if (Inputs.Jump.Check)
                    rb.SetSpeedY(Max(rb.velocity.y, varJumpSpeed));
                else
                    varJumpTimer = 0f;
            }

            // 开始跳
            if (Inputs.Jump.Pressed)
            {
                if (jumpGraceTimer > 0f)
                {
                    Jump();
                }
                else
                {
                    if (WallJumpCheck(1))
                    {
                        // 面朝右向右抓跳
                        if (facing == Facings.Right && Inputs.Grab.Check && Stamina > 0)
                        {
                            ClimbJump();
                        }
                        else
                        {
                            WallJump(-1);
                        }
                    }
                    else if (WallJumpCheck(-1))
                    {
                        // 面朝左向左抓跳
                        if (facing == Facings.Left && Inputs.Grab.Check && Stamina > 0)
                        {
                            ClimbJump();
                        }
                        else
                        {
                            WallJump(1);
                        }
                    }
                }
            }


            return StNormal;
        }

        private void Jump()
        {
            Inputs.Jump.ConsumeBuffer();
            if (moveX != 0)
                rb.AddSpeedX(JumpBoostH * moveX);
            rb.SetSpeedY(JumpSpeedV);
            jumpGraceTimer = 0;
            varJumpSpeed = JumpSpeedV;
            varJumpTimer = VarJumpTime;
            wallSlideTimer = WallSlideTime;

            sr.transform.localScale = new Vector2(0.7f, 1.3f);
            
            
            LightManager.Instance.CreateLight(transform.position);
        }

        private bool WallJumpCheck(int dir)
        {
            return CollideCheckBy(Vector2.right * dir * WallJumpCheckDistH, true);
        }
    }
}