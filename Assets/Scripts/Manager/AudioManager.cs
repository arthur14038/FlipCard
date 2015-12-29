using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class AudioManager : SingletonMonoBehavior<AudioManager>
{
	public AudioSource musicAudioSource;
	public AudioSource soundAudioSource;
	Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
	bool listenToToggle = false;

	public void SetListenToToggle(bool value)
	{
		listenToToggle = value;
	}

	public void MusicChangeValue(bool value)
	{
		if(listenToToggle)
		{
			PlayOneShot("Button_Click");
			PlayerPrefsManager.MusicSetting = value;
			if(value)
				ResumeMusic();
			else
				StopMusic();
		}
	}

	public void SoundChangeValue(bool value)
	{
		if(listenToToggle)
		{
			PlayOneShot("Button_Click");
			PlayerPrefsManager.SoundSetting = value;
		}
	}

	public void PlayOneShot(string clipName, float pitch = 1f)
	{
		if(PlayerPrefsManager.SoundSetting)
		{
			soundAudioSource.pitch = pitch;
			soundAudioSource.PlayOneShot(GetClip(clipName));
		}
	}

	public void SetVolume(float value, bool immediately)
	{
		if(musicAudioSource.volume != value)
		{
			if(immediately)
				musicAudioSource.volume = value;
			else
				musicAudioSource.DOFade(value, 0.2f);
		}
	}

	public void StopMusic()
	{
		SetVolume(0f, false);
	}

	public void ResumeMusic()
	{
		if(PlayerPrefsManager.MusicSetting)
			SetVolume(1f, false);
	}

	public void PlayMusic(string clipName, bool loop)
	{
		if(musicAudioSource.clip == null || musicAudioSource.clip.name != clipName || musicAudioSource.loop != loop)
		{
			musicAudioSource.clip = GetClip(clipName);
			musicAudioSource.loop = loop;
			musicAudioSource.Play();
		}
		if(PlayerPrefsManager.MusicSetting)
			SetVolume(1f, true);
		else
			SetVolume(0f, true);
	}

	AudioClip GetClip(string clipName)
	{
		AudioClip clip = null;
		if(audioClips.ContainsKey(clipName))
		{
			clip = audioClips[clipName];
		} else
		{
			clip = LoadClip(clipName);
			audioClips.Add(clipName, clip);
		}
		return clip;
	}

	AudioClip LoadClip(string clipName)
	{
		AudioClip clip = Resources.Load("Audio/" + clipName) as AudioClip;
		return clip;
	}
}
