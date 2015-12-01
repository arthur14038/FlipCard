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

	public static int OnePlayerProgress{
		get{
			return PlayerPrefs.GetInt("OnePlayerProgress", 0);
		}
		set{
			PlayerPrefs.SetInt("OnePlayerProgress", value);
		}
	}
}
