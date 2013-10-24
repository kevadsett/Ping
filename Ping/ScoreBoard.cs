using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Ping
{
	public enum Results { PlayerWin, AiWin, StillPlaying };
	
	public class ScoreBoard : SpriteUV
	{
		public int playerScore = 0, aiScore = 0;
		
		public ScoreBoard ()
		{
			this.TextureInfo = new TextureInfo();
			UpdateImage();
			
			this.Scale = this.TextureInfo.TextureSizef;
			this.Pivot = new Vector2(0.5f, 0.5f);
			this.Position = new Vector2(Director.Instance.GL.Context.GetViewport().Width / 2,
			                            Director.Instance.GL.Context.GetViewport().Height / 2);
		}
		
		private void UpdateImage() {
			Image image = new Image(ImageMode.Rgba, new ImageSize(110, 100), new ImageColor(0, 0, 0, 0));
			Font font = new Font(FontAlias.System, 50, FontStyle.Regular);
			image.DrawText(playerScore + " - " + aiScore, new ImageColor(255, 255, 255, 255), font, new ImagePosition(0,0));
			image.Decode();
			
			Texture2D texture = new Texture2D(110, 100, false, PixelFormat.Rgba);
			if(this.TextureInfo.Texture != null) {
				this.TextureInfo.Texture.Dispose();
			}
			this.TextureInfo.Texture = texture;
			texture.SetPixels(0, image.ToBuffer());
			font.Dispose();
			image.Dispose();
		}
		
		public void Clear() {
			playerScore = aiScore = 0;
			UpdateImage ();
		}
		
		public Results AddScore(bool player) {
			if(player) {
				playerScore ++;
			} else {
				aiScore ++;
			}
			if(playerScore > 3) return Results.PlayerWin;
			if(aiScore > 3) return Results.AiWin;
			
			this.UpdateImage();
			
			return Results.StillPlaying;
		}
	}
}

