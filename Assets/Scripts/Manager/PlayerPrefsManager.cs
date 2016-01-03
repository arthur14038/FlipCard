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

	public static int ClassicModeProgress{
		get{
			return PlayerPrefs.GetInt("ClassicModeProgress", 0);
		}
		set{
			PlayerPrefs.SetInt("ClassicModeProgress", value);
		}
	}
	
	public static int TimeModeProgress
	{
		get
		{
			return PlayerPrefs.GetInt("TimeModeProgress", -1);
		}
		set
		{
			PlayerPrefs.SetInt("TimeModeProgress", value);
		}
	}
}
