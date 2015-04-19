using System;
using System.Collections.Generic;

using Sce.PlayStation.Core.Audio;

namespace TheATeam
{
	public class AudioManager
	{
		// The players that control the playback of music and sound respectivly
		private static BgmPlayer musicPlayer = null;
		private static List<SoundPlayer> soundPlayers = new List<SoundPlayer>();

		// Gets if music is playing
		public static bool IsMusicPlaying {	get { return (musicPlayer != null) ? (musicPlayer.Status == BgmStatus.Playing) : false; } }
		
		// Checks to see if there are any sounds playing
		public static bool IsSoundPlaying
		{ 
			get
			{
				if (soundPlayers.Count <= 0)
				{
					return false;
				}
				for (int i = 0; i < soundPlayers.Count; i++)
				{
					if (soundPlayers[i].Status == SoundStatus.Playing)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Adds and or loads music and sound data to the manager
		
		public static bool AddMusic(string key, Bgm music)
		{
			return AssetManager<Bgm>.Add(key, music);
		}

		public static bool AddMusic(string key, string filename)
		{
			return AssetManager<Bgm>.Add(key, filename);
		}
		
		public static bool AddSound(string key, Sound sound)
		{
			return AssetManager<Sound>.Add(key, sound);
		}

		public static bool AddSound(string key, string filename)
		{
			return AssetManager<Sound>.Add(key, filename);
		}

		// Removes sound and music data from the manager and disposes of it

		public static bool RemoveMusic(string key)
		{
			return AssetManager<Bgm>.Remove(key);
		}

		public static bool RemoveSound(string key)
		{
			return AssetManager<Sound>.Remove(key);
		}

		// Checks that sound or music with the given key is loaded
		public static bool IsMusicLoaded(string key)
		{
			return AssetManager<Bgm>.IsAssetLoaded(key);
		}

		public static bool IsSoundLoaded(string key)
		{
			return AssetManager<Sound>.IsAssetLoaded(key);
		}

		// Tells the audio manager to playback sounds or music respectively, returns false if the specified key is not in the dictionary

		public static bool PlayMusic(string key, bool isLooping=true, float volume=1.0f, float playbackRate=1f)
		{
			if (!IsMusicLoaded(key) || IsMusicPlaying)
			{
				return false;
			}
			
			// Returns an instance of BgmPlayer to play this music data NOTE: is null until here
			musicPlayer = AssetManager<Bgm>.Get(key).CreatePlayer();
			musicPlayer.Volume = volume;
			musicPlayer.Loop = isLooping;
			musicPlayer.PlaybackRate = playbackRate;
			musicPlayer.Play();
			return true;
		}

		public static bool PlaySound(string key, bool isLooping=false, float volume=1f, float playbackRate=1f, float pan=0f)
		{
			if (!IsSoundLoaded(key))
			{
				return false;
			}
			
			// Returns an instance of the SoundPlayer to play this sound data NOTE: is null until here
			SoundPlayer soundPlayer = AssetManager<Sound>.Get(key).CreatePlayer();
			soundPlayer.Volume = volume;
			soundPlayer.Loop = isLooping;
			soundPlayer.PlaybackRate = playbackRate;
			soundPlayer.Pan = pan;
			soundPlayer.Play();
			soundPlayers.Add(soundPlayer);
			return true;
		}

		// Stops any music or sounds that are currently playing then disposes of the player
		public static void StopMusic()
		{
			if (musicPlayer == null)
			{	
				return;
			}
			
			musicPlayer.Stop();
			musicPlayer.Dispose();
			musicPlayer = null;
		}
		
		// Stops any sounds currently playing
		public static void StopSounds()
		{
			if (soundPlayers.Count <= 0)
			{
				return;
			}
			
			for (int i = 0; i < soundPlayers.Count; i++)
			{
				soundPlayers[i].Stop();
				soundPlayers[i].Dispose();
				soundPlayers.Remove(soundPlayers[i]);
			}
		}

		// Pause and resume the music
		public static void PauseMusic()
		{
			if (musicPlayer != null && musicPlayer.Status != BgmStatus.Paused)
			{
				musicPlayer.Pause();
			}
		}

		public static void ResumeMusic()
		{
			if (musicPlayer != null && musicPlayer.Status != BgmStatus.Playing)
			{
				musicPlayer.Resume();
			}
		}
		
		public static bool Initialise()
		{
			AddMusic("bgm", "bgm.mp3");
			AddSound("fire", "Fire.wav");
			AddSound("lose", "Lose.wav");
			AddSound("pickup", "Pickup.wav");
			AddSound("win", "Win.wav");
			return true;
		}
	}
}


