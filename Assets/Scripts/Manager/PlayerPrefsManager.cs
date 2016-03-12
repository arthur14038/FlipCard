using UnityEngine;
using System.Collections;

public class PlayerPrefsManager{
	public static bool SoundSetting{
		get{
			return PlayerPrefs.GetInt("SoundSetting", 1) == 1 ? true : false;
		}
		set{
			PlayerPrefs.SetInt("SoundSetting", value ? 1 : 0);
		}
	}

	public static bool MusicSetting{
		get{
			return PlayerPrefs.GetInt("MusicSetting", 1) == 1 ? true : false;
		}
		set{
			PlayerPrefs.SetInt("MusicSetting", value ? 1 : 0);
		}
	}

	public static int UnlockMode
	{
		get
		{
			return PlayerPrefs.GetInt("UnlockMode", 0);
		}
		set
		{
			PlayerPrefs.SetInt("UnlockMode", value);
		}
	}

	public static string InfiniteScore
	{
		get
		{
			return PlayerPrefs.GetString("InfiniteScore", "");
		}
		set
		{
			PlayerPrefs.SetString("InfiniteScore", value);
		}
	}

	public static bool FirstInfiniteAchievement
	{
		get
		{
			return PlayerPrefs.GetInt("FirstInfiniteAchievement", 0) == 0 ? false : true;
		}
		set
		{
			PlayerPrefs.SetInt("FirstInfiniteAchievement", value ? 1 : 0);
		}
	}

	public static bool SecondInfiniteAchievement
	{
		get
		{
			return PlayerPrefs.GetInt("SecondInfiniteAchievement", 0) == 0 ? false : true;
		}
		set
		{
			PlayerPrefs.SetInt("SecondInfiniteAchievement", value ? 1 : 0);
		}
	}
}
