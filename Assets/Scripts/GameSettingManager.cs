using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public enum CardArrayLevel{TwoByThree = 0, ThreeByFour, FourByFive, FiveBySix}
public enum GameMode { LimitTime = 1, LimitMove, Competition, Cooperation}
public class GameSettingManager{
	static Dictionary<CardArrayLevel, CardArraySetting> cardArraySettings = new Dictionary<CardArrayLevel, CardArraySetting>();
	static Dictionary<CardArrayLevel, TimeModeSetting> timeModeSettings = new Dictionary<CardArrayLevel, TimeModeSetting>();
	static Dictionary<CardArrayLevel, CompetitionModeSetting> competitionModeSettings = new Dictionary<CardArrayLevel, CompetitionModeSetting>();
	public static CardArrayLevel currentLevel;
	public static GameMode currentMode;

	public static void LoadData()
	{
		string jsonString = ((TextAsset)Resources.Load("CardArraySetting")).text;
		List<CardArraySetting> tmp = JsonConvert.DeserializeObject<List<CardArraySetting>>(jsonString);
		foreach(CardArraySetting s in tmp)
		{
			if(!cardArraySettings.ContainsKey(s.level))
			{
				s.realFirstPosition = new Vector2(s.firstPosition[0], s.firstPosition[1]);
				cardArraySettings.Add(s.level, s);
			}
		}

		jsonString = ((TextAsset)Resources.Load("TimeModeSetting")).text;
		List<TimeModeSetting> tmp1 = JsonConvert.DeserializeObject<List<TimeModeSetting>>(jsonString);
		foreach(TimeModeSetting s in tmp1)
		{
			if(!timeModeSettings.ContainsKey(s.level))
				timeModeSettings.Add(s.level, s);
		}

		jsonString = ((TextAsset)Resources.Load("CompetitionModeSetting")).text;
		List<CompetitionModeSetting> tmp2 = JsonConvert.DeserializeObject<List<CompetitionModeSetting>>(jsonString);
		foreach(CompetitionModeSetting s in tmp2)
		{
			if(!competitionModeSettings.ContainsKey(s.level))
				competitionModeSettings.Add(s.level, s);
		}
	}

	public static CardArraySetting GetCurrentCardArraySetting()
	{
		if(cardArraySettings.ContainsKey(currentLevel))
			return cardArraySettings[currentLevel];
		else
			return null;
	}

	public static TimeModeSetting GetCurrentTimeModeSetting()
	{
		if(timeModeSettings.ContainsKey(currentLevel))
			return timeModeSettings[currentLevel];
		else
			return null;
	}

	public static CompetitionModeSetting GetCurrentCompetitionModeSetting()
	{
		if(competitionModeSettings.ContainsKey(currentLevel))
			return competitionModeSettings[currentLevel];
		else
			return null;
	}
}

public class CardArraySetting{
	public CardArrayLevel level;
	public float edgeLength;
	public int column;
	public int row;
	public float cardGap;
	public float[] firstPosition;
	public Vector2 realFirstPosition;
}

public class TimeModeSetting
{
	public CardArrayLevel level;
	public float gameTime;
	public float showCardTime;
	public float awardTime;
	public int matchAddScore;
	public int mismatchReduceScore;
	public int comboAddScore;
	public int unknownCardShowRound;
	public int feverTimeComboThreshold;
}

public class CompetitionModeSetting
{
	public CardArrayLevel level;
	public int matchAddScore;
	public int mismatchReduceScore;
	public int comboAddScore;
	public int goldCardCount;
}