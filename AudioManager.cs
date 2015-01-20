using System;
using System.Collections.Generic;

using Sce.PlayStation.Core.Audio;

namespace PairedGame
{
	public static class AudioManager
	{
		//Contains all the sounds that are currently loaded
		private static Dictionary<string, Sound> soundList = new Dictionary<string, Sound>();
		
		//Contains all of the music that is currently loaded
		private static Dictionary<string, Bgm> musicList = new Dictionary<string, Bgm>();

		//The players that control the playback of music and sound respectivly
		private static BgmPlayer musicPlayer = null;
		private static List<SoundPlayer> soundPlayers = new List<SoundPlayer>();

		//Adds loaded music and sound data to the manager
		public static void AddMusic(string key, Bgm music)
		{
			musicList.Add(key, music);
		}

		public static void AddMusic(string filename, string key)
		{
			if(!musicList.ContainsKey(key))
			{
				Bgm music = new Bgm(filename);
				musicList.Add(key, music);
			}
		}
		
		public static void AddSound(string key, Sound sound)
		{
			soundList.Add(key, sound);
		}

		public static void AddSound(string filename, string key)
		{
			if(!soundList.ContainsKey(key))
			{
				Sound sound = new Sound(filename);
				soundList.Add(key, sound);
			}
		}

		//Removes sound and music data from the manager
		//and disposes of the data

		public static void RemoveMusic(string key)
		{
			musicList[key].Dispose();
			musicList.Remove(key);
		}

		public static void RemoveSound(string key)
		{
			soundList[key].Dispose();
			soundList.Remove(key);
		}

		//Checks that sound or music with the given key is loaded
		public static bool IsMusicLoaded(string key)
		{
			return musicList.ContainsKey(key);
		}

		public static bool IsSoundLoaded(string key)
		{
			return soundList.ContainsKey(key);
		}

		//Tells the audio manager to playback sounds or music respectivly

		//returns false if the specified key is not in the dictionary

		public static bool PlayMusic(string key, bool isLooping, float volume,
								float playbackRate)
		{
			if(musicList.ContainsKey(key))
			{
				if(IsMusicPlaying())
					return false;
				//Returns an instance of BgmPlayer to play this music data NOTE: is null until here
				musicPlayer = musicList[key].CreatePlayer();
				musicPlayer.Volume = volume;
				musicPlayer.Loop = isLooping;
				musicPlayer.PlaybackRate = playbackRate;
				musicPlayer.Play();
				return true;	
			}
			else
			{
				return false;
			}
		}

		public static bool PlaySound(string key, bool isLooping, float volume,
							float playbackRate, float pan)
		{
			if(soundList.ContainsKey(key))
			{
				//Returns an instance of the SoundPlayer to play this sound data NOTE: is null until here
				SoundPlayer soundPlayer = soundList[key].CreatePlayer();
				soundPlayer.Volume = volume;
				soundPlayer.Loop = isLooping;
				soundPlayer.PlaybackRate = playbackRate;
				soundPlayer.Pan = pan;
				soundPlayer.Play();
				soundPlayers.Add(soundPlayer);
				return true;
			}
			else
			{
				return false;
			}
		}

		//Stops any music or sounds that are currently playing then disposes of the player
		public static void StopMusic()
		{
			if(musicPlayer != null)
			{	
				musicPlayer.Stop();
				musicPlayer.Dispose();
				musicPlayer = null;
			}	
		}
		/*
		* Stops any sounds currently playing
		*/
		public static void StopSounds()
		{
			for(int i = 0; i < soundPlayers.Count; i++)
			{
				soundPlayers[i].Stop();
				soundPlayers[i].Dispose();
				soundPlayers.Remove(soundPlayers[i]);
			}
		}

		//Gets if music or sound is playing
		public static bool IsMusicPlaying()
		{
			if(musicPlayer != null)
			{
				if(musicPlayer.Status == BgmStatus.Stopped
							|| musicPlayer.Status == BgmStatus.Paused)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		//Checks to see if there are any sounds playing
		public static bool IsSoundPlaying()
		{
			if(soundPlayers.Count != 0)
			{
				for(int i = 0; i < soundPlayers.Count; i++)
				{
					if(soundPlayers[i].Status == SoundStatus.Playing)
					{
						return true;
					}
				}
				return false;
			}
			else
			{
				return false;
			}
		}

		//checks if the music is paused
		public static bool IsMusicPaused()
		{
			if(musicPlayer != null && musicPlayer.Status == BgmStatus.Paused)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		//Pause and resume the music
		public static void PauseMusic()
		{
			if(musicPlayer != null && musicPlayer.Status != BgmStatus.Paused)
			{
				musicPlayer.Pause();
			}
		}

		public static void ResumeMusic()
		{
			if(musicPlayer != null && musicPlayer.Status != BgmStatus.Playing)
			{
				musicPlayer.Resume();
			}
		}
	}
}


