using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.Physics2D;

namespace Ping
{
	public class Ball : SpriteUV
	{
		private PhysicsBody _physicsBody;
		
		public const float BALL_VELOCITY = 7.0f;
		
		public Ball (PhysicsBody physicsBody)
		{
			_physicsBody = physicsBody;
			
			this.TextureInfo = new TextureInfo(new Texture2D("/Application/images/ball.png", false));
			this.Scale = this.TextureInfo.TextureSizef;
			this.Pivot = new Sce.PlayStation.Core.Vector2(0.5f,0.5f);
			this.Position = new Sce.PlayStation.Core.Vector2(
				Director.Instance.GL.Context.GetViewport().Width / 2 - Scale.X / 2,
				Director.Instance.GL.Context.GetViewport().Height / 2 - Scale.Y / 2
				);
			
			// Avoid starting at right angles
			System.Random rand = new System.Random();
			float angle = (float)rand.Next(0, 360);
			
			if ((angle % 90) <= 25) angle+= (float)rand.Next (0, 1) > 0.5 ? 25.0f : -25.0f;
			this._physicsBody.Velocity = new Vector2(0.0f, BALL_VELOCITY).Rotate(PhysicsUtility.GetRadian(angle));
			
			Scheduler.Instance.ScheduleUpdateForTarget(this, 0, false);
		}
		
		public override void Update (float dt)
		{
			this.Position = _physicsBody.Position * PingPhysics.PtoM;
			
			// Don't want the ball to bounce from side to side too much
			Vector2 normalisedVelocity = _physicsBody.Velocity.Normalize();
			if(System.Math.Abs(normalisedVelocity.Y) < 0.2f) {
				if(normalisedVelocity.Y < 0) { //going down
					normalisedVelocity.Y -= 0.2f;
				} else { // going up
					normalisedVelocity.Y += 0.2f;
				}
			}
			
			_physicsBody.Velocity = normalisedVelocity * BALL_VELOCITY;
		}
		
		~Ball() {
			this.TextureInfo.Texture.Dispose();
			this.TextureInfo.Dispose();
		}
	}
}

