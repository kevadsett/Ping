using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Input;

namespace Ping
{
	public class GameOverScene : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private TextureInfo _textureInfo;
		private Texture2D _texture;
		
		public GameOverScene (bool win)
		{
			this.Camera.SetViewFromViewport();
			if(win) {
				_texture = new Texture2D("/Application/images/winner.png", false);
			} else {
				_texture = new Texture2D("/Application/images/loser.png", false);
			}
			_textureInfo = new TextureInfo(_texture);
			SpriteUV titleScreen = new SpriteUV(_textureInfo);
			titleScreen.Scale = _textureInfo.TextureSizef;
			titleScreen.Pivot = new Vector2(0.5f, 0.5f);
			titleScreen.Position = new Vector2(Director.Instance.GL.Context.GetViewport().Width / 2,
			                                   Director.Instance.GL.Context.GetViewport().Height / 2);
			this.AddChild(titleScreen);
			
			Scheduler.Instance.ScheduleUpdateForTarget(this, 0, false);
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			int touchCount = Touch.GetData(0).Count;
			if(touchCount > 0 || Input2.GamePad0.Cross.Press) {
				Director.Instance.ReplaceScene(new TitleScene());
			}
		}
		
		~GameOverScene() {
			this._texture.Dispose();
			this._textureInfo.Dispose();
		}
	}
}

