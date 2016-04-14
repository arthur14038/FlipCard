using UnityEngine;
using System;

public class PlayerPrefsManager{
	static string encodeKey = "FlipCard";
	static string encodeMoniIV = "Pleasure";
	static string encodeScoreIV = "VeryGood";

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

	public static int InfiniteScore
	{
		get
		{
			string encodedString = PlayerPrefs.GetString("InfiniteScore", null);
			if(string.IsNullOrEmpty(encodedString))
			{
				return 0;
			} else
			{
				string decodeString = EncodeTool.GetDecodedBase64(encodedString, encodeKey, encodeScoreIV);
				return int.Parse(decodeString);
			}
		}
		set
		{
			string encodedString = EncodeTool.GetEncodedBase64(value.ToString(), encodeKey, encodeScoreIV);
			PlayerPrefs.SetString("InfiniteScore", encodedString);
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

	public static int MoniCount
	{
		get
		{
			string encodedString = PlayerPrefs.GetString("MoniCount", null);
			if(string.IsNullOrEmpty(encodedString))
			{
				return 0;
			}else
			{
				string decodeString = EncodeTool.GetDecodedBase64(encodedString, encodeKey, encodeMoniIV);
				return int.Parse(decodeString);
			}
        }
		set
		{
			string encodedString = EncodeTool.GetEncodedBase64(value.ToString(), encodeKey, encodeMoniIV);
			PlayerPrefs.SetString("MoniCount", encodedString);
		}
	}

	public static string EquipedThemeId
	{
		get
		{
			return PlayerPrefs.GetString("EquipedThemeId", "Theme_00");
		}
		set
		{
			PlayerPrefs.SetString("EquipedThemeId", value);
        }
	}

	public static string EquipedCardFaceId
	{
		get
		{
			return PlayerPrefs.GetString("EquipedCardFaceId", "CardFace_000");
		}
		set
		{
			PlayerPrefs.SetString("EquipedCardFaceId", value);
		}
	}

	public static string EquipedCardBackId
	{
		get
		{
			return PlayerPrefs.GetString("EquipedCardBackId", "CardBack_000");
		}
		set
		{
			PlayerPrefs.SetString("EquipedCardBackId", value);
		}
	}

	public static int OwnedTheme
	{
		get
		{
			return PlayerPrefs.GetInt("OwnedTheme", 261);
		}
		set
		{
			PlayerPrefs.SetInt("OwnedTheme", value);
		}
	}

	public static bool CanShowAwardAd
	{
		get
		{
			DateTime lastTimeShow = new DateTime(long.Parse(PlayerPrefs.GetString("CanShowAwardAd", "0")));
			TimeSpan timeSpan = DateTime.Now - lastTimeShow;
			Debug.Log(lastTimeShow);
			if(timeSpan.TotalHours > 8)
				return true;
			else
				return false;
        }
		set
		{
			if(!value)
				PlayerPrefs.SetString("CanShowAwardAd", DateTime.Now.Ticks.ToString());
		}
	}
}
