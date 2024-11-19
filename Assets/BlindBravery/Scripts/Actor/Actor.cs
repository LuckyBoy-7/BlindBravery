// using Lucky.Kits;
//
// namespace Lucky.Platform2D.Actor
// {
//     public class Actor : ManagedBehaviour
//     {
// 		public bool OnGround(int downCheck = 1)
// 		{
// 			return base.CollideCheck<Solid>(this.Position + Vector2.UnitY * (float)downCheck) || (!this.IgnoreJumpThrus && base.CollideCheckOutside<JumpThru>(this.Position + Vector2.UnitY * (float)downCheck));
// 		}
//
// 		public bool OnGround(Vector2 at, int downCheck = 1)
// 		{
// 			Vector2 position = this.Position;
// 			this.Position = at;
// 			bool flag = this.OnGround(downCheck);
// 			this.Position = position;
// 			return flag;
// 		}
//
// 		public Vector2 ExactPosition
// 		{
// 			get
// 			{
// 				return this.Position + this.movementCounter;
// 			}
// 		}
//
// 		public Vector2 PositionRemainder
// 		{
// 			get
// 			{
// 				return this.movementCounter;
// 			}
// 		}
//
// 		public void ZeroRemainderX()
// 		{
// 			this.movementCounter.X = 0f;
// 		}
//
// 		public void ZeroRemainderY()
// 		{
// 			this.movementCounter.Y = 0f;
// 		}
//
// 		public override void Update()
// 		{
// 			base.Update();
// 			this.LiftSpeed = Vector2.Zero;
// 			if (this.liftSpeedTimer > 0f)
// 			{
// 				this.liftSpeedTimer -= Engine.DeltaTime;
// 				if (this.liftSpeedTimer <= 0f)
// 				{
// 					this.lastLiftSpeed = Vector2.Zero;
// 				}
// 			}
// 		}
//
// 		public Vector2 LiftSpeed
// 		{
// 			get
// 			{
// 				if (this.currentLiftSpeed == Vector2.Zero)
// 				{
// 					return this.lastLiftSpeed;
// 				}
// 				return this.currentLiftSpeed;
// 			}
// 			set
// 			{
// 				this.currentLiftSpeed = value;
// 				if (value != Vector2.Zero && this.LiftSpeedGraceTime > 0f)
// 				{
// 					this.lastLiftSpeed = value;
// 					this.liftSpeedTimer = this.LiftSpeedGraceTime;
// 				}
// 			}
// 		}
//
// 		public void ResetLiftSpeed()
// 		{
// 			this.currentLiftSpeed = (this.lastLiftSpeed = Vector2.Zero);
// 			this.liftSpeedTimer = 0f;
// 		}
//
// 		public bool MoveH(float moveH, Collision onCollide = null, Solid pusher = null)
// 		{
// 			this.movementCounter.X = this.movementCounter.X + moveH;
// 			int num = (int)Math.Round((double)this.movementCounter.X, MidpointRounding.ToEven);
// 			if (num != 0)
// 			{
// 				this.movementCounter.X = this.movementCounter.X - (float)num;
// 				return this.MoveHExact(num, onCollide, pusher);
// 			}
// 			return false;
// 		}
//
// 		public bool MoveV(float moveV, Collision onCollide = null, Solid pusher = null)
// 		{
// 			this.movementCounter.Y = this.movementCounter.Y + moveV;
// 			int num = (int)Math.Round((double)this.movementCounter.Y, MidpointRounding.ToEven);
// 			if (num != 0)
// 			{
// 				this.movementCounter.Y = this.movementCounter.Y - (float)num;
// 				return this.MoveVExact(num, onCollide, pusher);
// 			}
// 			return false;
// 		}
//
// 		public bool MoveHExact(int moveH, Collision onCollide = null, Solid pusher = null)
// 		{
// 			Vector2 vector = this.Position + Vector2.UnitX * (float)moveH;
// 			int num = Math.Sign(moveH);
// 			int num2 = 0;
// 			while (moveH != 0)
// 			{
// 				Solid solid = base.CollideFirst<Solid>(this.Position + Vector2.UnitX * (float)num);
// 				if (solid != null)
// 				{
// 					this.movementCounter.X = 0f;
// 					if (onCollide != null)
// 					{
// 						onCollide(new CollisionData
// 						{
// 							Direction = Vector2.UnitX * (float)num,
// 							Moved = Vector2.UnitX * (float)num2,
// 							TargetPosition = vector,
// 							Hit = solid,
// 							Pusher = pusher
// 						});
// 					}
// 					return true;
// 				}
// 				num2 += num;
// 				moveH -= num;
// 				base.X += (float)num;
// 			}
// 			return false;
// 		}
//
// 		public bool MoveVExact(int moveV, Collision onCollide = null, Solid pusher = null)
// 		{
// 			Vector2 vector = this.Position + Vector2.UnitY * (float)moveV;
// 			int num = Math.Sign(moveV);
// 			int num2 = 0;
// 			while (moveV != 0)
// 			{
// 				Platform platform = base.CollideFirst<Solid>(this.Position + Vector2.UnitY * (float)num);
// 				if (platform != null)
// 				{
// 					this.movementCounter.Y = 0f;
// 					if (onCollide != null)
// 					{
// 						onCollide(new CollisionData
// 						{
// 							Direction = Vector2.UnitY * (float)num,
// 							Moved = Vector2.UnitY * (float)num2,
// 							TargetPosition = vector,
// 							Hit = platform,
// 							Pusher = pusher
// 						});
// 					}
// 					return true;
// 				}
// 				if (moveV > 0 && !this.IgnoreJumpThrus)
// 				{
// 					platform = base.CollideFirstOutside<JumpThru>(this.Position + Vector2.UnitY * (float)num);
// 					if (platform != null)
// 					{
// 						this.movementCounter.Y = 0f;
// 						if (onCollide != null)
// 						{
// 							onCollide(new CollisionData
// 							{
// 								Direction = Vector2.UnitY * (float)num,
// 								Moved = Vector2.UnitY * (float)num2,
// 								TargetPosition = vector,
// 								Hit = platform,
// 								Pusher = pusher
// 							});
// 						}
// 						return true;
// 					}
// 				}
// 				num2 += num;
// 				moveV -= num;
// 				base.Y += (float)num;
// 			}
// 			return false;
// 		}
//
// 		public void MoveTowardsX(float targetX, float maxAmount, Collision onCollide = null)
// 		{
// 			float num = Calc.Approach(this.ExactPosition.X, targetX, maxAmount);
// 			this.MoveToX(num, onCollide);
// 		}
//
// 		public void MoveTowardsY(float targetY, float maxAmount, Collision onCollide = null)
// 		{
// 			float num = Calc.Approach(this.ExactPosition.Y, targetY, maxAmount);
// 			this.MoveToY(num, onCollide);
// 		}
//
// 		public void MoveToX(float toX, Collision onCollide = null)
// 		{
// 			this.MoveH(toX - this.ExactPosition.X, onCollide, null);
// 		}
//
// 		public void MoveToY(float toY, Collision onCollide = null)
// 		{
// 			this.MoveV(toY - this.ExactPosition.Y, onCollide, null);
// 		}
//
// 		public void NaiveMove(Vector2 amount)
// 		{
// 			this.movementCounter += amount;
// 			int num = (int)Math.Round((double)this.movementCounter.X);
// 			int num2 = (int)Math.Round((double)this.movementCounter.Y);
// 			this.Position += new Vector2((float)num, (float)num2);
// 			this.movementCounter -= new Vector2((float)num, (float)num2);
// 		}
//
// 		public Collision SquishCallback;
//
// 		public bool TreatNaive;
//
// 		private Vector2 movementCounter;
//
// 		public bool IgnoreJumpThrus;
//
// 		public bool AllowPushing = true;
//
// 		public float LiftSpeedGraceTime = 0.16f;
//
// 		private Vector2 currentLiftSpeed;
//
// 		private Vector2 lastLiftSpeed;
//
// 		private float liftSpeedTimer;
// 	}
// }