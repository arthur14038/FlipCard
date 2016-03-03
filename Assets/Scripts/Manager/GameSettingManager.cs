using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public enum LevelDifficulty{EASY = 0, NORMAL, HARD, CRAZY, Lock}
public enum GameMode { FlipCard = 1, Competition}
public class GameSettingManager{
	static Dictionary<LevelDifficulty, CardArraySetting> cardArraySettings = new Dictionary<LevelDifficulty, CardArraySetting>();
	static Dictionary<LevelDifficulty, CompetitionModeSetting> competitionModeSettings = new Dictionary<LevelDifficulty, CompetitionModeSetting>();
	static Dictionary<int, FlipCardGameSetting> flipCardSettings = new Dictionary<int, FlipCardGameSetting>();
	static Dictionary<int, FlipCardArraySetting> flipCardArraySettings = new Dictionary<int, FlipCardArraySetting>();
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
		
		jsonString = ((TextAsset)Resources.Load("CompetitionModeSetting")).text;
		List<CompetitionModeSetting> tmp2 = JsonConvert.DeserializeObject<List<CompetitionModeSetting>>(jsonString);
		foreach(CompetitionModeSetting s in tmp2)
		{
			if(!competitionModeSettings.ContainsKey(s.level))
				competitionModeSettings.Add(s.level, s);
		}

		jsonString = ((TextAsset)Resources.Load("FlipCardSetting")).text;
		List<FlipCardGameSetting> tmp3 = JsonConvert.DeserializeObject<List<FlipCardGameSetting>>(jsonString);
		foreach(FlipCardGameSetting s in tmp3)
		{
			if(!flipCardSettings.ContainsKey(s.level))
				flipCardSettings.Add(s.level, s);
		}

		jsonString = ((TextAsset)Resources.Load("FlipCardArraySetting")).text;
		List<FlipCardArraySetting> tmp4 = JsonConvert.DeserializeObject<List<FlipCardArraySetting>>(jsonString);
		foreach(FlipCardArraySetting s in tmp4)
		{
			s.realCardPosition = new Vector2[s.cardPosition.Length];
			for(int i = 0 ; i < s.cardPosition.Length ; ++i)
			{
				string[] posInfo = s.cardPosition[i].Split(',');
				Vector2 realPos = new Vector2(float.Parse(posInfo[0]), float.Parse(posInfo[1]));
				s.realCardPosition[i] = realPos;
            }
			if(!flipCardArraySettings.ContainsKey(s.level))
				flipCardArraySettings.Add(s.level, s);
		}
	}

	public static CardArraySetting GetCurrentCardArraySetting()
	{
		if(cardArraySettings.ContainsKey(currentLevel))
			return cardArraySettings[currentLevel];
		else
			return null;
	}

	public static FlipCardGameSetting GetFlipCardGameSetting(int level)
	{
		if(flipCardSettings.ContainsKey(level))
			return flipCardSettings[level];
		else
			return null;
	}

	public static FlipCardArraySetting GetFlipCardArraySetting(int level)
	{
		if(flipCardArraySettings.ContainsKey(level))
			return flipCardArraySettings[level];
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
	public LevelDifficulty level;
	public float edgeLength;
	public int column;
	public int row;
	public float cardGap;
	public float[] firstPosition;
	public Vector2 realFirstPosition;
}

public class CompetitionModeSetting
{
	public LevelDifficulty level;
	public int matchAddScore;
	public int goldCardCount;
}

public class FlipCardGameSetting
{
	public int level;
	public int round;
	public int cardCount;
	public float showCardTime;
    public float levelTime;
	public int[] questionCardAppearRound;
	public int[] specialCardAppearRound;
	public int specialCardType;
	public int questionCardCount;
}

public class FlipCardArraySetting
{
	public int level;
	public float cardSize;
	public string[] cardPosition;
	public Vector2[] realCardPosition;
}