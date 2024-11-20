using BlindBravery.Enums;
using Lucky.Framework;
using Lucky.Framework.Inputs_;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using UnityEngine;
using static Lucky.Kits.Utilities.MathUtils;
using StateMachine = Lucky.Kits.Utilities.StateMachine;
using Timer = Lucky.Kits.Utilities.Timer;

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

        public float Width => 8;
        public float Height => 11;
        public Vector2 TopCenter => transform.position + Vector3.up * Height;
        public Vector2 BottomAhead => transform.position + Vector3.right * Width / 2 * (int)facing;
        public Vector2 CenterAhead => BottomAhead + Vector2.up * Height / 2;

        private RaycastHit2D[] collideCheckHelper = new RaycastHit2D[1];
        public int moveX;
        private Rigidbody2D rb;
        private StateMachine stateMachine;
        private Facings facing = Facings.Right;
        private Vector2 lastAim = Vector2.right;
        private Vector2 preSpeed;
        public bool onGround;
        public Vector2 Right => AngleToVector(sr.transform.eulerAngles.z, 1);

        public static readonly Color NormalHairColor = ColorUtils.HexToColor("AC3232");
        public static readonly Color UsedHairColor = ColorUtils.HexToColor("44B7FF");
        private const float SlopeCheckDistY = 4;
        public bool nearSlope;
        private Vector2 slopePerpendicularRight;

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
            // onGround = CollideCheckBy(Vector2.down);
            onGround = CollideCheckBy(Vector2.down) && (nearSlope || rb.velocity.y <= 0);
            SlopeCheck();
            //
            preSpeed = rb.velocity;
            // if (onGround)
            //     MoveV(-202202);

            // 可能是斜坡
            float distX = 4;
            // if ((CollideCheckSlopeBy(Vector2.right * distX) || CollideCheckSlopeBy(Vector2.left * distX)))
            if ((CollideCheckSlopeBy(Vector2.right * distX) || CollideCheckSlopeBy(Vector2.left * distX)))
            {
                hitbox = capsuleCollider;
                boxCollider.enabled = false;
                capsuleCollider.enabled = true;
            }
            else
            {
                hitbox = boxCollider;
                boxCollider.enabled = true;
                capsuleCollider.enabled = false;
            }

            if (hopWaitX != 0)
            {
                // 变向或者下落就取消climbhop
                if (Sign(rb.velocity.x) == -hopWaitX || rb.velocity.y < 0f)
                {
                    hopWaitX = 0;
                }
                else if (!CollideCheckBy(Vector3.right * hopWaitX))
                {
                    print(123);
                    rb.AddSpeedX(hopWaitXSpeed);
                    hopWaitX = 0;
                }
            }
        }

        private void SlopeCheck()
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, SlopeCheckDistY, 1 << LayerMask.NameToLayer("Slope"));
            // print(raycastHit2D.collider);
            Debug.DrawRay(transform.position, Vector3.down * SlopeCheckDistY, Color.blue);
            nearSlope = false;
            if (raycastHit2D.collider)
            {
                // print(raycastHit2D.collider.name);
                slopePerpendicularRight = raycastHit2D.normal.TurnRight();
                nearSlope = true;
                Debug.DrawRay(raycastHit2D.point, slopePerpendicularRight * SlopeCheckDistY, Color.red);
            }
        }

        public override void Render()
        {
            base.Render();

            UpdateSprite();

            // 体力不足闪红
            bool flash = Timer.OnInterval(0.1f);
            if (IsTired && flash)
            {
                sr.color = Color.red;
            }
        }

        private void UpdateSprite()
        {
            sr.transform.SetScaleX(Sign2(sr.transform.localScale.x) * Approach(Abs(sr.transform.localScale.x), 1f, 1.75f * Timer.DeltaTime()));
            sr.transform.SetScaleY(Sign2(sr.transform.localScale.y) * Approach(Abs(sr.transform.localScale.y), 1f, 1.75f * Timer.DeltaTime()));

            // 翻转图像
            sr.transform.SetScaleX(Abs(sr.transform.localScale.x) * (int)facing);

            // 旋转
            float rotateAngle = 0;
            float angle = SignedAngle(Vector2.right, slopePerpendicularRight);
            if (nearSlope && -75 < angle && angle < 75)
                rotateAngle = angle;
            float start = sr.transform.eulerAngles.z;
            if (start > 180) // 因为这个z -> [0, 360]
                start -= 360;
            sr.transform.eulerAngles = new Vector3(0, 0, Approach(start, rotateAngle, 250 * Timer.DeltaTime()));
        }

        private bool PointCollideCheck(Vector2 pos)
        {
            LayerMask layerMask = 1 << LayerMask.NameToLayer("Solid") | 1 << LayerMask.NameToLayer("Slope");
            return Physics2D.OverlapPoint(pos, layerMask);
        }

        private bool CollideCheckBy(Vector2 offset, bool ignoreSlope = false)
        {
            LayerMask layerMask = 1 << LayerMask.NameToLayer("Solid");
            if (!ignoreSlope)
                layerMask |= 1 << LayerMask.NameToLayer("Slope");
            return Physics2D.OverlapBox(hitbox.transform.position + (Vector3)hitbox.offset + (Vector3)offset, hitbox.bounds.size * 0.99f, 0, layerMask);
        }

        private bool CollideCheckSlopeBy(Vector2 offset)
        {
            LayerMask layerMask = 1 << LayerMask.NameToLayer("Slope");
            return Physics2D.OverlapBox(hitbox.transform.position + (Vector3)hitbox.offset + (Vector3)offset, hitbox.bounds.size * 0.99f, 0, layerMask);
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (preSpeed.x != rb.velocity.x)
                OnCollideH();
            else if (preSpeed.y != rb.velocity.y)
                OnCollideV();

            LightManager.Instance.CreateLight(other.GetContact(0).point);
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
            // todo : 晚点修修(实在修不好可能就干脆Sprite放大点了)
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
    }
}