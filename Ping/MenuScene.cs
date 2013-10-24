using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

using Sce.PlayStation.HighLevel.UI;

namespace Ping
{
	public class MenuScene : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private Sce.PlayStation.HighLevel.UI.Scene _uiScene;
		
		public MenuScene ()
		{
			this.Camera.SetViewFromViewport();
			Panel dialog = new Panel();
			
			dialog.Width = Director.Instance.GL.Context.GetViewport().Width;
			dialog.Height = Director.Instance.GL.Context.GetViewport().Height;
			
			ImageBox ib = new ImageBox();
			ib.Width = dialog.Width;
			ib.Image = new ImageAsset("/Application/images/title.png", false);
			ib.Height = dialog.Height;
			ib.SetPosition(0.0f, 0.0f);
			
			Button playButton = new Button();
			playButton.Name = "buttonPlay";
			playButton.Text = "Play Game";
			playButton.Width = 300;
			playButton.Height = 50;
			playButton.Alpha = 0.8f;
			playButton.SetPosition(dialog.Width/2 - playButton.Width / 2, 200.0f);
			playButton.TouchEventReceived += (sender, e) => {
				Director.Instance.ReplaceScene(new GameScene());
			};
			
			Button menuButton = new Button();
			menuButton.Name = "buttonMenu";
			menuButton.Text = "Main Menu";
			menuButton.Width = 300;
			menuButton.Height = 50;
			menuButton.Alpha = 0.8f;
			menuButton.SetPosition(dialog.Width/2 - playButton.Width / 2, 250.0f);
			menuButton.TouchEventReceived += (sender, e) => {
				Director.Instance.ReplaceScene(new TitleScene());
			};
			
			dialog.AddChildLast(ib);
			dialog.AddChildLast(playButton);
			dialog.AddChildLast(menuButton);
			_uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			_uiScene.RootWidget.AddChildLast(dialog);
			UISystem.SetScene(_uiScene);
			Scheduler.Instance.ScheduleUpdateForTarget(this, 0, false);
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			UISystem.Update(Touch.GetData(0));
		}
		
		public override void Draw ()
		{
			base.Draw ();
			UISystem.Render();
		}
		
		~MenuScene() {
			
		}
	}
}

