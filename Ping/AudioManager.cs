using System;
using Sce.PlayStation.Core.Audio;

namespace Ping
{
	public class AudioManager
	{
		private Bgm _titleSong;
		private Bgm _mainSong;
		private BgmPlayer _songPlayer;
		
		public AudioManager ()
		{
			System.Diagnostics.Debug.WriteLine("Audio manager constructor");
			_titleSong = new Bgm("/Application/audio/titlesong.mp3");
			_mainSong = new Bgm("/Application/audio/wikistep.mp3");
			
			if(_songPlayer != null) {
				_songPlayer.Dispose();
			}
			_songPlayer = _titleSong.CreatePlayer();
			_songPlayer.Loop = true;
			_songPlayer.Play ();
			
		}
		
		
		~AudioManager() {
			_songPlayer.Stop();
			_songPlayer.Dispose();
			_songPlayer = null;
		}
		
		public void changeSong (bool playMainSong) {
			if(playMainSong) {
				if(_songPlayer != null) {
					_songPlayer.Dispose();
				}
				_songPlayer = _mainSong.CreatePlayer();
			} else {
				if(_songPlayer != null) {
					_songPlayer.Dispose();
				}
				_songPlayer = _titleSong.CreatePlayer();
			}
			_songPlayer.Play ();
		}
		
	}
}

