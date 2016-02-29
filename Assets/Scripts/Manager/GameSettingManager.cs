using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public enum LevelDifficulty{EASY = 0, NORMAL, HARD, CRAZY, Lock}
public enum GameMode { LimitTime = 1, Classic, Competition, Cooperation}
public class GameSettingManager{
	static Dictionary<LevelDifficulty, CardArraySetting> cardArraySettings = new Dictionary<LevelDifficulty, CardArraySetting>();
	static Dictionary<LevelDifficulty, TimeModeSetting> timeModeSettings = new Dictionary<LevelDifficulty, TimeModeSetting>();
	static Dictionary<LevelDifficulty, CompetitionModeSetting> competitionModeSettings = new Dictionary<LevelDifficulty, CompetitionModeSetting>();
	static Dictionary<LevelDifficulty, ClassicModeSetting> classicModeSettings = new Dictionary<LevelDifficulty, ClassicModeSetting>();
	static List<SinglePlayerLevel> singlePlayerLevelList;
	public static LevelDifficulty currentLevel;
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

	public static List<SinglePlayerLevel> GetSinglePlayerLevel(GameMode mode)
	{
		List<SinglePlayerLevel> tmp = new List<SinglePlayerLevel>();
		foreach(SinglePlayerLevel level in singlePlayerLevelList)
			if(level.gameMode == mode)
				tmp.Add(level);
		return tmp;
	}
}

public class CardArraySetting{
	public LevelDifficulty level;
	public float edgeLength;
	public int column;
	public int row;
	public float cardGap;
	public float[] firstPosition;
	public Vector2 realFirstPosition;
}

public class TimeModeSetting
{
	public LevelDifficulty level;
	public float gameTime;
	public float showCardTime;
	public float awardTime;
	public int matchAddScore;
    public int targetRound;
	public int targetFeverTimeCount;
	public int targetScore;
}

public class CompetitionModeSetting
{
	public LevelDifficulty level;
	public int matchAddScore;
	public int goldCardCount;
}

public class ClassicModeSetting
{
	public LevelDifficulty level;
	public int excellentMove;
	public int targetMove;
	public float targetTime;
}

public class SinglePlayerLevel
{
	public LevelDifficulty gameLevel;
	public GameMode gameMode;
	public string firstInformationTitle;
	public string secondInformationTitle;
	public string lockInstruction;
	public string headerColor;
}