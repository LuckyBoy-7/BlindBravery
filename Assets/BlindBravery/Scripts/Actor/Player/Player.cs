using System;
using System.Collections;
using Lucky.Framework;
using Lucky.Kits.Extensions;
using Lucky.Kits;
using Lucky.Framework.Inputs_;
using BlindBravery.Enums;
using Lucky.Kits.Utilities;
using UnityEngine;
using static Lucky.Kits.Utilities.MathUtils;

namespace BlindBravery.Actor.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class Player : ManagedBehaviour
    {
        // debug
        [Header("DebugInfo")] public Vector2 SpeedDebug;
        public Facings FacingsDebug = Facings.Right;
        public int StateName;
        public bool IsLoggingState;
        public float MaxSpeedX;

        [Header("Debug")] public bool IsInfiniteStamina;

        [Header("Others")] public Collider2D hitbox;
        public Collider2D boxCollider;
        public Collider2D capsuleCollider;
        public SpriteRenderer sr;
        private const int StNormal = 0;
        private const int StClimb = 1;

        public int moveX;
        private Rigidbody2D rb;
        private StateMachine stateMachine;
        private RaycastHit2D[] collideCheckHelper = new RaycastHit2D[1];
        private Facings facing = Facings.Right;
        private Vector2 lastAim = Vector2.right;
        private Vector2 preSpeed;
        public bool onGround;
        public Vector2 Right => AngleToVector(sr.transform.eulerAngles.z, 1);
        private bool hasChangedRotationInThisFrame = false;

        public static readonly Color NormalHairColor = ColorUtils.HexToColor("AC3232");
        public static readonly Color UsedHairColor = ColorUtils.HexToColor("44B7FF");
        public float distX;
        public float upY;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            stateMachine = new StateMachine(this);
            stateMachine.SetCallbacks(StNormal, "Normal", null, null, NormalUpdate);
            stateMachine.SetCallbacks(StClimb, "Climb", ClimbBegin, null, ClimbUpdate);
            stateMachine.State = StNormal;
        }

        protected override void ManagedUpdate()
        {
            // debug
            if (IsInfiniteStamina)
            {
                Stamina = ClimbMaxStamina;
            }


            // 狼跳
            if (onGround)
            {
                jumpGraceTimer = JumpGraceTime;
            }
            else if (jumpGraceTimer > 0f)
            {
                jumpGraceTimer -= Timer.DeltaTime();
            }

            if (wallSlideDir != 0)
            {
                wallSlideTimer = Max(wallSlideTimer - Timer.DeltaTime(), 0f);
                wallSlideDir = 0;
            }

            // 偷体力
            if (wallBoostTimer > 0f)
            {
                wallBoostTimer -= Timer.DeltaTime();
                if (moveX == wallBoostDir)
                {
                    rb.SetSpeedX(WallJumpSpeedH * moveX);
                    Stamina += ClimbJumpStaminaCost;
                    wallBoostTimer = 0f;
                }
            }

            // 贴地回体力
            if (onGround)
            {
                Stamina = ClimbMaxStamina;
                wallSlideTimer = WallSlideTime;
            }

            // 跳跃上升时间
            if (varJumpTimer > 0)
                varJumpTimer -= Timer.DeltaTime();


            // 强制移动(或者说模拟输入)
            if (forceMoveXTimer > 0f)
            {
                forceMoveXTimer -= Timer.DeltaTime();
                moveX = forceMoveX;
            }
            else
            {
                moveX = Inputs.MoveX.Value;
            }

            // 当前按键方向
            if (Inputs.MoveX.Value != 0 || Inputs.MoveY.Value != 0)
                lastAim = new Vector2(Inputs.MoveX.Value, Inputs.MoveY.Value).normalized;

            // retention
            if (wallSpeedRetentionTimer > 0f)
            {
                // 逆向了直接取消Retention
                if (Sign(rb.velocity.x) == -Sign(wallSpeedRetained))
                {
                    wallSpeedRetentionTimer = 0f;
                }
                else if (!CollideCheckBy(Vector2.right * Sign(wallSpeedRetained)))
                {
                    // 返还速度
                    rb.SetSpeedX(wallSpeedRetained);
                    wallSpeedRetentionTimer = 0f;
                }
                else
                {
                    wallSpeedRetentionTimer -= Timer.DeltaTime();
                }
            }

            // logic view
            if (moveX != 0 && stateMachine.State != StClimb)
            {
                Facings facings = (Facings)moveX;
                facing = facings;
            }

            // todo: =============================================================================
            // component
            stateMachine.Update();
            UpdateSprite();

            // debug
            SpeedDebug = rb.velocity;
            stateMachine.Log = IsLoggingState;
            FacingsDebug = facing;
            StateName = stateMachine.State;
            MaxSpeedX = Max(MaxSpeedX, Abs(rb.velocity.x));
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            // 是否在地面
            onGround = CollideCheckBy(Vector2.down) && rb.velocity.y <= 0;
            //
            preSpeed = rb.velocity;
            hasChangedRotationInThisFrame = false;
            // if (onGround)
            //     MoveV(-202202);

            // 可能是斜坡
            // float distX = 4;
            if ((CollideCheckBy(Vector2.right * distX) || CollideCheckBy(Vector2.left * distX)))
            {
                hitbox = capsuleCollider;
                boxCollider.enabled = false;
                capsuleCollider.enabled = true;
            }
            // else
            // {
            //     hitbox = boxCollider;
            //     boxCollider.enabled = true;
            //     capsuleCollider.enabled = false;
            // }
        }

        public override void Render()
        {
            base.Render();


            // 体力不足闪红
            bool flash = Timer.OnInterval(0.1f);
            if (IsTired && flash)
            {
                sr.color = Color.red;
            }


            // 翻转图像
            sr.transform.SetScaleX(Abs(sr.transform.localScale.x) * (int)facing);
        }

        private void UpdateSprite()
        {
            sr.transform.SetScaleX(Sign2(sr.transform.localScale.x) * Approach(Abs(sr.transform.localScale.x), 1f, 1.75f * Timer.DeltaTime()));
            sr.transform.SetScaleY(Sign2(sr.transform.localScale.y) * Approach(Abs(sr.transform.localScale.y), 1f, 1.75f * Timer.DeltaTime()));
            sr.transform.eulerAngles = new Vector3(0, 0, Approach(sr.transform.eulerAngles.z, 0, 100 * Timer.DeltaTime()));
        }

        private bool CollideCheckBy(Vector2 offset)
        {
            return Physics2D.OverlapBox(
                hitbox.transform.position + (Vector3)hitbox.offset + (Vector3)offset, hitbox.bounds.size * 0.95f, 0, 1 << LayerMask.NameToLayer("Solid")
            );
            var prePos = hitbox.transform.position;
            hitbox.transform.position += (Vector3)offset;
            Physics2D.SyncTransforms();
            bool res = hitbox.IsTouchingLayers(1 << LayerMask.NameToLayer("Solid"));
            hitbox.transform.position = prePos;
            Physics2D.SyncTransforms();
            return res;
            // return hitbox.Cast(vec.normalized, new ContactFilter2D() { layerMask =  }, collideCheckHelper, vec.magnitude) > 0;
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (preSpeed.x != rb.velocity.x)
                OnCollideH();
            else if (preSpeed.y != rb.velocity.y)
                OnCollideV();
        }

        private void OnCollideV()
        {
            if (stateMachine.State != StClimb)
            {
                if (preSpeed.y < 0)
                {
                    float k = Min(-preSpeed.y / MaxFallSpeedY, 1f);
                    sr.transform.localScale = new Vector3(Lerp(1f, 1.4f, k), Lerp(1f, 0.8f, k));
                }
            }
            // rb.SetSpeedY(0);
        }

        private void OnCollideH()
        {
            if (wallSpeedRetentionTimer <= 0f)
            {
                wallSpeedRetained = preSpeed.x;
                wallSpeedRetentionTimer = WallSpeedRetentionTime;
            }
        }

        /// <summary>
        /// 因为rb的movePosition不能及时更新, 然后又没什么好的方法, 只能这样了
        /// </summary>
        /// <param name="x"></param>
        private void MoveH(float x)
        {
            // rb.MovePosition(rb.position + Vector2.right * x);
            if (hitbox.Cast(Vector2.right * Sign(x), collideCheckHelper, Abs(x)) > 0)
            {
                RaycastHit2D hit = collideCheckHelper[0];
                transform.AddPositionX(hit.distance * Sign(x) - Sign(x) * SmallValue);
                Physics2D.SyncTransforms();
                return;
            }

            transform.AddPositionX(x);
            Physics2D.SyncTransforms();
        }

        private void MoveV(float y)
        {
            // rb.MovePosition(rb.position + Vector2.up * y);
            if (hitbox.Cast(Vector2.up * Sign(y), collideCheckHelper, Abs(y)) > 0)
            {
                RaycastHit2D hit = collideCheckHelper[0];
                transform.AddPositionY(hit.distance * Sign(y) - Sign(y) * SmallValue);
                Physics2D.SyncTransforms();
                return;
            }

            transform.AddPositionY(y);
            Physics2D.SyncTransforms();
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            // if (other.otherCollider is not CircleCollider2D)
            // return;
            ContactPoint2D contact = other.GetContact(0);
            // if (this.Dist(contact.point) <= 2)
            //     return;

            Vector2 normal = contact.normal;
            float angle = VectorAngle(normal);
            if (15 < angle && angle < 165)
            {
                // rb.SetRotation(angle - 90);
                sr.transform.eulerAngles = new Vector3(0, 0, angle - 90);
                // transform.position = contact.point;
                hasChangedRotationInThisFrame = true;
            }
        }


        private void OnCollisionExit2D(Collision2D other)
        {
            bool justLeaveSlope = sr.transform.eulerAngles.z != 0 && CollideCheckBy(Vector2.down * 3) && rb.velocity.y <= 1;
            if (justLeaveSlope)
            {
                print("PushDown");
            }

            StartCoroutine(Wait());
            this.WaitForTwoFrameToExecution(
                () => { }
            );
        }

        IEnumerator Wait()
        {
            yield return new WaitForFixedUpdate();
        }
    }
}