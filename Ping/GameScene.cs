using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.Physics2D;
using Sce.PlayStation.Core.Audio;
using System.Collections.Generic;

namespace Ping
{
	public class GameScene : Scene
	{
		private Paddle _player, _ai;
		public static Ball ball;
		
		public float shakeAmount = 10;
		private PingPhysics _physics;
		private ScoreBoard _scoreboard;
		private List<SoundPlayer> _pingHitSoundPlayers;
		private List<Sound> _pingSounds;
		
		private static Boolean DEBUG_BOUNDINGBOXES = false;
		
		public GameScene ()
		{
			if(AppMain.am != null) {
				AppMain.am.changeSong(true);
			}
			this.Camera.SetViewFromViewport();
			_physics = new PingPhysics();
			
			ball = new Ball(_physics.SceneBodies[(int)PingPhysics.BODIES.Ball]);
			_player = new Paddle(Paddle.PaddleType.PLAYER, _physics.SceneBodies[(int)PingPhysics.BODIES.Player]);
			_ai = new Paddle(Paddle.PaddleType.AI, _physics.SceneBodies[(int)PingPhysics.BODIES.Ai]);
			_scoreboard = new ScoreBoard();
			
			this.AddChild(_scoreboard);
			this.AddChild(ball);
			this.AddChild(_player);
			this.AddChild(_ai);
			
			// this is a debug routine that will draw the physics bounding box around the objects
			if(DEBUG_BOUNDINGBOXES) {
				this.AdHocDraw += () => {
					var bottomLeftPlayer = _physics.SceneBodies[(int)PingPhysics.BODIES.Player].AabbMin;
					var topRightPlayer = _physics.SceneBodies[(int)PingPhysics.BODIES.Player].AabbMax;
					Director.Instance.DrawHelpers.DrawBounds2Fill(
						new Bounds2(bottomLeftPlayer * PingPhysics.PtoM, topRightPlayer * PingPhysics.PtoM));
					
					var bottomLeftAi = _physics.SceneBodies[(int)PingPhysics.BODIES.Ai].AabbMin;
					var topRightAi = _physics.SceneBodies[(int)PingPhysics.BODIES.Ai].AabbMax;
					Director.Instance.DrawHelpers.DrawBounds2Fill(
						new Bounds2(bottomLeftAi * PingPhysics.PtoM, topRightAi * PingPhysics.PtoM));
					
					var bottomLeftBall = _physics.SceneBodies[(int)PingPhysics.BODIES.Ball].AabbMin;
					var topRightBall = _physics.SceneBodies[(int)PingPhysics.BODIES.Ball].AabbMax;
					Director.Instance.DrawHelpers.DrawBounds2Fill(
						new Bounds2(bottomLeftBall * PingPhysics.PtoM, topRightBall * PingPhysics.PtoM));
					
				};
			}
			
			// load up the sound fx and create a player
			_pingSounds = new List<Sound>();
			_pingHitSoundPlayers = new List<SoundPlayer>();
			for (int i = 0; i < 5; i++) {
				_pingSounds.Add(new Sound("/Application/audio/hit0" + i + ".wav"));
				_pingHitSoundPlayers.Add(_pingSounds[i].CreatePlayer());
			}
			
			Scheduler.Instance.ScheduleUpdateForTarget(this, 0, false);
		}
		
		private void ResetBall() {
			// Move ball to screen centre and release in a random direction
			_physics.SceneBodies[(int)PingPhysics.BODIES.Ball].Position = 
				new Vector2(Director.Instance.GL.Context.GetViewport().Width / 2,
				            Director.Instance.GL.Context.GetViewport().Height / 2) / PingPhysics.PtoM;
			
			System.Random rand = new System.Random();
			float angle = (float)rand.Next(0, 360);
			
			if((angle%90) <= 15) {
				angle += (float)rand.Next() > 0.5 ? 15.0f : -15.0f;
			}
			
			_physics.SceneBodies[(int)PingPhysics.BODIES.Ball].Velocity = 
				new Vector2(0.0f, 5.0f).Rotate (PhysicsUtility.GetRadian(angle));
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			if(this.shakeAmount > 0) {
				this.shakeAmount -= 0.2f;
			} else {
				this.shakeAmount = 0;
			}
			if(_physics.QueryContact(0, 1) || _physics.QueryContact(0, 2)) {
				this.shakeAmount = 10;
			}
			Random rand = new Random();
			this.Camera2D.SetViewFromWidthAndBottomLeft(
				Director.Instance.GL.Context.GetViewport().Width,
				new Vector2 (rand.Next(-(int)this.shakeAmount, (int)this.shakeAmount), rand.Next(-(int)this.shakeAmount, (int)this.shakeAmount)));
			if(Input2.GamePad0.Select.Press) {
				Director.Instance.ReplaceScene(new MenuScene());
			}
			
			// We don't need these but sadly, the Simulate call does.
			Vector2 dummy1 = new Vector2();
			Vector2 dummy2 = new Vector2();
			
			// Update the physics simulation
			_physics.Simulate(-1, dummy1, dummy2);
			
			// check the ball hit a paddle, play a sound
			if(_physics.QueryContact((uint)PingPhysics.BODIES.Ball, (uint)PingPhysics.BODIES.Player) ||
			   _physics.QueryContact((uint)PingPhysics.BODIES.Ball, (uint)PingPhysics.BODIES.Ai)) {
				bool soundIsPlaying = false;
				for(int i = 0; i < _pingSounds.Count; i++) {
					if(_pingHitSoundPlayers[i].Status == SoundStatus.Playing) {
						soundIsPlaying = true;
						break;
					}
				}
				if(!soundIsPlaying) {
					_pingHitSoundPlayers[(int)System.Math.Round((double)rand.Next(0, _pingSounds.Count))].Play();
				}
			}
			
			// check if the ball went off screen and update score accordingly
			Results result = Results.StillPlaying;
			bool scored = false;
			
			if(ball.Position.Y > Director.Instance.GL.Context.GetViewport().Height + ball.Scale.Y/2) {
				result = _scoreboard.AddScore(true);
				scored = true;
			}
			if(ball.Position.Y < 0 - ball.Scale.Y/2) {
				result = _scoreboard.AddScore(false);
				scored = true;
			}
			
			// is there a winner? Show the appropriate screen
			if(result == Results.AiWin) {
				Director.Instance.ReplaceScene(new GameOverScene(false));
			}
			if(result == Results.PlayerWin) {
				Director.Instance.ReplaceScene(new GameOverScene(true));
			}
			
			// if someone scored but the game's not over, reset the ball
			if(scored == true) {
				ResetBall();
			}
			
			// finally, a sanity check to make sure the ball didn't leave the field
			PhysicsBody ballPB = _physics.SceneBodies[(int)PingPhysics.BODIES.Ball];
			
			if(ballPB.Position.X < -(ball.Scale.X/2f)/PingPhysics.PtoM ||
			   ballPB.Position.X > (Director.Instance.GL.Context.GetViewport().Width) / PingPhysics.PtoM) {
				ResetBall();
			}
		}
		
		~GameScene() {
			foreach(SoundPlayer player in _pingHitSoundPlayers) {
				player.Dispose();
			}
		}
	}
}

