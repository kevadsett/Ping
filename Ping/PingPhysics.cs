using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.Physics2D;

namespace Ping
{
	public class PingPhysics : PhysicsScene
	{
		// PixelsToMetres
		public const float PtoM = 50.0f;
		private const float BALL_RADIUS = 35.0f/2f;
		private const float PADDLE_WIDTH = 125.0f;
		private const float PADDLE_HEIGHT = 38.0f;
		private float _screenWidth;
		private float _screenHeight;
		
		public enum BODIES { Ball = 0, Player, Ai, LeftBumper, RightBumper };
		
		public PingPhysics ()
		{
			_screenWidth = Director.Instance.GL.Context.GetViewport().Width;
			_screenHeight = Director.Instance.GL.Context.GetViewport().Height;
			
			// turn gravity off
			this.InitScene();
			this.Gravity = new Vector2(0.0f, 0.0f);
			
			// Set the screen boundaries + 2m or 100 pixel
			this.SceneMin = new Vector2(-100f, -100f) / PtoM;
			this.SceneMax = new Vector2(_screenWidth + 100.0f, _screenHeight + 100.0f) / PtoM;
			
			// turn up the BOUNCE!
			this.RestitutionCoeff = 1.0f;
			
			this.NumBody = 5; // Ball, 2 paddles, 2 bumpers
			this.NumShape = 3; // One of each
			
			// creating the ball
			this.SceneShapes[0] = new PhysicsShape(BALL_RADIUS / PtoM);
			this.SceneBodies[0] = new PhysicsBody(this.SceneShapes[0], 0.1f);
			this.SceneBodies[0].ShapeIndex = 0;
			this.SceneBodies[0].ColFriction = 0.01f;
			this.SceneBodies[0].Position = new Vector2(_screenWidth/2, _screenHeight/2) / PtoM;
			
			// Paddle shape
			Vector2 box = new Vector2(PADDLE_WIDTH / 2f / PtoM, PADDLE_HEIGHT / 2f / PtoM);
			this.SceneShapes[1] = new PhysicsShape(box);
			
			// Player paddle
			this.SceneBodies[1] = new PhysicsBody(this.SceneShapes[1], 1.0f);
			this.SceneBodies[1].Position = new Vector2(_screenWidth/2f, 0f + PADDLE_HEIGHT / 2 + 10f) / PtoM;
			this.SceneBodies[1].Rotation = 0;
			this.SceneBodies[1].ShapeIndex = 1;
			
			// AI paddle
			this.SceneBodies[2] = new PhysicsBody(this.SceneShapes[1], 1.0f);
			float aiX = (_screenWidth / 2f) / PtoM;
			float aiY = (_screenHeight - PADDLE_HEIGHT / 2 - 10f) / PtoM;
			this.SceneBodies[2].Position = new Vector2(aiX, aiY);
			this.SceneBodies[2].Rotation = 0;
			this.SceneBodies[2].ShapeIndex = 1;
			
			// shape for the bumpers so the ball doesn't go off-screen
			this.SceneShapes[2] = new PhysicsShape((new Vector2(1.0f, _screenHeight)) / PtoM);
			
			// Left bumper
			this.SceneBodies[3] = new PhysicsBody(this.SceneShapes[2], PhysicsUtility.FltMax);
			this.SceneBodies[3].Position = new Vector2(0, _screenHeight/2f) / PtoM;
			this.SceneBodies[3].ShapeIndex = 2;
			this.SceneBodies[3].Rotation = 0;
			this.SceneBodies[3].SetBodyStatic();
			
			// Right bumper
			this.SceneBodies[4] = new PhysicsBody(this.SceneShapes[2], PhysicsUtility.FltMax);
			this.SceneBodies[4].Position = new Vector2(_screenWidth - 10.0f, _screenHeight/2f) / PtoM;
			this.SceneBodies[4].ShapeIndex = 2;
			this.SceneBodies[4].Rotation = 0;
			this.SceneBodies[4].SetBodyStatic();
		}
	}
}

