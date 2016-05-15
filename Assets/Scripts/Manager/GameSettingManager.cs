using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public enum GameMode { FlipCard = 1, Competition, PickCard}
public class GameSettingManager{
	static Dictionary<int, CompetitionModeSetting> competitionModeSettings = new Dictionary<int, CompetitionModeSetting>();
	static Dictionary<int, FlipCardGameSetting> flipCardSettings = new Dictionary<int, FlipCardGameSetting>();
	static Dictionary<int, FlipCardArraySetting> flipCardArraySettings = new Dictionary<int, FlipCardArraySetting>();
	static Dictionary<int, PickGameSetting> pickGameSettings = new Dictionary<int, PickGameSetting>();
	static Dictionary<int, PickCardArraySetting> pickCardArraySettings = new Dictionary<int, PickCardArraySetting>();
	public static GameMode currentMode;
	public static int currentCardCount;

	public static void LoadData()
	{
		string jsonString = ((TextAsset)Resources.Load("CompetitionModeSetting")).text;
		List<CompetitionModeSetting> tmp2 = JsonConvert.DeserializeObject<List<CompetitionModeSetting>>(jsonString);
		foreach(CompetitionModeSetting s in tmp2)
		{
			if(!competitionModeSettings.ContainsKey(s.cardCount))
				competitionModeSettings.Add(s.cardCount, s);
		}

		jsonString = ((TextAsset)Resources.Load("FlipCardGameSetting")).text;
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
			if(!flipCardArraySettings.ContainsKey(s.cardCount))
				flipCardArraySettings.Add(s.cardCount, s);
		}

		jsonString = ((TextAsset)Resources.Load("PickCardArraySetting")).text;
		List<PickCardArraySetting> tmp5 = JsonConvert.DeserializeObject<List<PickCardArraySetting>>(jsonString);
		foreach(PickCardArraySetting s in tmp5)
		{
			s.realCardPosition = new Vector2[s.cardPosition.Length];
			for(int i = 0 ; i < s.cardPosition.Length ; ++i)
			{
				string[] posInfo = s.cardPosition[i].Split(',');
				Vector2 realPos = new Vector2(float.Parse(posInfo[0]), float.Parse(posInfo[1]));
				s.realCardPosition[i] = realPos;
			}
			if(!pickCardArraySettings.ContainsKey(s.cardCount))
				pickCardArraySettings.Add(s.cardCount, s);
		}

		jsonString = ((TextAsset)Resources.Load("PickGameSetting")).text;
		List<PickGameSetting> tmp6 = JsonConvert.DeserializeObject<List<PickGameSetting>>(jsonString);
		foreach(PickGameSetting s in tmp6)
		{
			if(!pickGameSettings.ContainsKey(s.cardCount))
				pickGameSettings.Add(s.level, s);
		}
	}
	
	public static FlipCardGameSetting GetFlipCardGameSetting(int level)
	{
		if(flipCardSettings.ContainsKey(level))
			return flipCardSettings[level];
		else
			return null;
	}

	public static FlipCardArraySetting GetFlipCardArraySetting(int cardCount)
	{
		if(flipCardArraySettings.ContainsKey(cardCount))
			return flipCardArraySettings[cardCount];
		else
			return null;
	}

	public static CompetitionModeSetting GetCurrentCompetitionModeSetting()
	{
		if(competitionModeSettings.ContainsKey(currentCardCount))
			return competitionModeSettings[currentCardCount];
		else
			return null;
	}

	public static PickGameSetting GetPickGameSetting(int level)
	{
		if(pickGameSettings.ContainsKey(level))
			return pickGameSettings[level];
		else
	        return null;
	}

	public static PickCardArraySetting GetPickCardArraySetting(int cardCount)
	{
		if(pickCardArraySettings.ContainsKey(cardCount))
			return pickCardArraySettings[cardCount];
		else
			return null;
	}
}

public class CompetitionModeSetting
{
	public int cardCount;
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
}

public class FlipCardArraySetting
{
	public int cardCount;
	public float cardSize;
	public string[] cardPosition;
	public Vector2[] realCardPosition;
}

public class PickGameSetting
{
	public int level;
	public int cardCount;
	public int targetCardCount;
	public float showCardTime;
}

public class PickCardArraySetting
{
	public int cardCount;
	public float cardSize;
	public string[] cardPosition;
	public Vector2[] realCardPosition;
}