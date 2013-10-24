using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.Physics2D;

namespace Ping
{
	public class Paddle : SpriteUV
	{
		public enum PaddleType {PLAYER, AI};
		
		private PaddleType _type;
		private PhysicsBody _physicsBody;
		private float _fixedY;
		
		public Paddle (PaddleType type, PhysicsBody physicsBody)
		{
			this._type = type;
			this._physicsBody = physicsBody;
			
			this.TextureInfo = new TextureInfo(new Texture2D("/Application/images/paddle.png", false));
			this.Scale = TextureInfo.TextureSizef;
			this.Pivot = new Vector2(0.5f, 0.5f);
			
			if(_type == PaddleType.AI) {
				this.Position = new Vector2(
					Director.Instance.GL.Context.GetViewport().Width / 2 - this.Scale.X / 2,
					this.Scale.Y / 2 + 10);
			} else { 
				this.Position = new Vector2(
					Director.Instance.GL.Context.GetViewport().Width / 2 - this.Scale.X / 2,
					Director.Instance.GL.Context.GetViewport().Height - this.Scale.Y / 2 - 10);
			}
			
			// cache the vertical position so we can reset after physics
			_fixedY = _physicsBody.Position.Y;
			
			// start with a minor amount of movement
			_physicsBody.Force = new Vector2(-10.0f, 0);
			
			Scheduler.Instance.ScheduleUpdateForTarget(this, 0, false);
		}
		
		// Fix sprite bounding box to current position (unused)
		private void ClampBoundingBox() {
			var bbBl = new Vector2(this.Position.X - this.Scale.X / 2, this.Position.Y - this.Scale.Y / 2) / PingPhysics.PtoM;
			var bbTr = new Vector2(this.Position.X + this.Scale.X / 2, this.Position.Y + this.Scale.Y / 2) / PingPhysics.PtoM;
			_physicsBody.AabbMin = bbBl;
			_physicsBody.AabbMax = bbTr;
		}
		
		public override void Update (float dt)
		{
			// Reset rotation to avoid spinning on collision
			_physicsBody.Rotation = 0.0f;
			
			if(_type == PaddleType.PLAYER) {
				if(Input2.GamePad0.Left.Down) {
					_physicsBody.Force = new Vector2(-30.0f, 0.0f);
				}
				if(Input2.GamePad0.Right.Down) {
					_physicsBody.Force = new Vector2(30.0f, 0.0f);
				}
			} else if (_type == PaddleType.AI){
				if(System.Math.Abs(GameScene.ball.Position.X - this.Position.X) <= this.Scale.Y / 2) {
					_physicsBody.Force = new Vector2(0.0f, 0.0f);
				} else if (GameScene.ball.Position.X < this.Position.X) {
					_physicsBody.Force = new Vector2(-20.0f, 0.0f);
				} else if (GameScene.ball.Position.X > this.Position.X) {
					_physicsBody.Force = new Vector2(20.0f, 0.0f);
				}
			}
			
			// prevent vertical movement on collision
			if(_physicsBody.Position.Y != _fixedY) {
				_physicsBody.Position = new Vector2(_physicsBody.Position.X, _fixedY);
			}
			
			this.Position = _physicsBody.Position * PingPhysics.PtoM;
		}
		
		~Paddle() {
			this.TextureInfo.Texture.Dispose();
			this.TextureInfo.Dispose();
		}
	}
}

