using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public enum CardArrayLevel{TwoByThree = 0, ThreeByFour, FourByFive, FiveBySix}
public enum GameMode { LimitTime = 0, LimitMove, Competition, Cooperation}
public class CardArrayManager{
	static Dictionary<CardArrayLevel, CardArraySetting> settings = new Dictionary<CardArrayLevel, CardArraySetting>();
	public static CardArrayLevel currentLevel;
	public static GameMode currentMode;

	public static void LoadData()
	{
		string jsonString = ((TextAsset)Resources.Load("CardArraySetting")).text;
		List<CardArraySetting> tmp = JsonConvert.DeserializeObject<List<CardArraySetting>>(jsonString);
		foreach(CardArraySetting s in tmp)
		{
			if(!settings.ContainsKey(s.level))
			{
				s.realFirstPosition = new Vector2(s.firstPosition[0], s.firstPosition[1]);
				settings.Add(s.level, s);
			}
		}
	}

	public static CardArraySetting GetCurrentLevelSetting()
	{
		if(settings.ContainsKey(currentLevel))
			return settings[currentLevel];
		else
			return null;
	}
}

public class CardArraySetting{
	public CardArrayLevel level;
	public float edgeLength;
	public int column;
	public int row;
	public GameMode mode;
	public float cardGap;
	public float[] firstPosition;
	public Vector2 realFirstPosition;
	public float gameTime;
	public float showCardTime;
	public float awardTime;
}