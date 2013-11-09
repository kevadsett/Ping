using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Input;

namespace Ping
{
	public class TitleScene : Scene
	{
		private TextureInfo _textureInfo;
		private Texture2D _texture;
		
		public TitleScene ()
		{
			if(AppMain.am == null) {
				AppMain.am = new AudioManager();
			} else {
				AppMain.am.changeSong(false);
			}
			this.Camera.SetViewFromViewport();
			_texture = new Texture2D("/Application/images/title.png", false);
			_textureInfo = new TextureInfo(_texture);
			SpriteUV titleScreen = new SpriteUV(_textureInfo);
			titleScreen.Scale = _textureInfo.TextureSizef;
			titleScreen.Pivot = new Vector2(0.5f, 0.5f);
			ImageRect viewPort = Director.Instance.GL.Context.GetViewport();
			titleScreen.Position = new Vector2(viewPort.Width/2, viewPort.Height/2);
			this.AddChild(titleScreen);
			
			Vector4 origColour = titleScreen.Color;
			titleScreen.Color = new Vector4(0,0,0,0);
			var tintAction = new TintTo(origColour, 10.0f);
			ActionManager.Instance.AddAction(tintAction, titleScreen);
			tintAction.Run();
			
			Scheduler.Instance.ScheduleUpdateForTarget(this, 0, false);
			
			// Clear any queued clicks so we don't immediately exit if coming in from the menu
			Touch.GetData (0).Clear();
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			var touches = Touch.GetData(0).ToArray();
			if((touches.Length > 0 && touches[0].Status == TouchStatus.Down) || Input2.GamePad0.Cross.Press) {
				Director.Instance.ReplaceScene(new MenuScene());
			}
		}
		
		~TitleScene() {
			_texture.Dispose();
			_textureInfo.Dispose();
		}
	}
}

