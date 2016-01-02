using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public enum CardArrayLevel{TwoByThree = 0, ThreeByFour, FourByFive, FiveBySix}
public enum GameMode { LimitTime = 1, Classic, Competition, Cooperation}
public class GameSettingManager{
	static Dictionary<CardArrayLevel, CardArraySetting> cardArraySettings = new Dictionary<CardArrayLevel, CardArraySetting>();
	static Dictionary<CardArrayLevel, TimeModeSetting> timeModeSettings = new Dictionary<CardArrayLevel, TimeModeSetting>();
	static Dictionary<CardArrayLevel, CompetitionModeSetting> competitionModeSettings = new Dictionary<CardArrayLevel, CompetitionModeSetting>();
	static Dictionary<CardArrayLevel, ClassicModeSetting> classicModeSettings = new Dictionary<CardArrayLevel, ClassicModeSetting>();
	static List<SinglePlayerLevel> singlePlayerLevelList;
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

		jsonString = ((TextAsset)Resources.Load("ClassicModeSetting")).text;
		List<ClassicModeSetting> tmp3 = JsonConvert.DeserializeObject<List<ClassicModeSetting>>(jsonString);
		foreach(ClassicModeSetting s in tmp3)
		{
			if(!classicModeSettings.ContainsKey(s.level))
				classicModeSettings.Add(s.level, s);
		}
		
		jsonString = ((TextAsset)Resources.Load("SinglePlayerLevel")).text;
		singlePlayerLevelList = JsonConvert.DeserializeObject<List<SinglePlayerLevel>>(jsonString);
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

	public static ClassicModeSetting GetCurrentClassicModeSetting()
	{
		if(classicModeSettings.ContainsKey(currentLevel))
			return classicModeSettings[currentLevel];
		else
			return null;
	}

	public static List<SinglePlayerLevel> GetAllSinglePlayerLevel()
	{
		return singlePlayerLevelList;
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
	public int gradeGap;
}

public class CompetitionModeSetting
{
	public CardArrayLevel level;
	public int matchAddScore;
	public int mismatchReduceScore;
	public int comboAddScore;
	public int goldCardCount;
}

public class ClassicModeSetting
{
	public CardArrayLevel level;
	public int gradeGap;
	public int excellentMove;
}

public class SinglePlayerLevel
{
	public int requireProgress;
	public CardArrayLevel gameLevel;
	public GameMode gameMode;
	public string showContent;
	public string levelTitle;
	public string lockInstruction;
	public string headerColor;
}