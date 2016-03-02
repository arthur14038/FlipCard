using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public enum UseLayout {LevelDifficulty, Infinite}
public class TestCardArray : MonoBehaviour {
	public Transform cardParent;
	public UseLayout useLayout = UseLayout.LevelDifficulty;
	public LevelDifficulty difficulty;
	public int totalCardCount = 6;
	public float cardSize = 192f;
	public float gap = 50f;
	public Vector2 center = Vector2.zero;
	List<RectTransform> cardList = new List<RectTransform>();
	
	public void LoadAndShowCards()
	{
		GameSettingManager.LoadData();
		switch(useLayout)
		{
			case UseLayout.LevelDifficulty:
				GameSettingManager.currentLevel = difficulty;
				CardArraySetting setting = GameSettingManager.GetCurrentCardArraySetting();
				int cardCount = setting.row * setting.column;
				LoadCard(cardCount);
				Vector2[] cardPos = GetCardPosition(setting);
				for(int i = 0 ; i < cardList.Count ; ++i)
				{
					if(i < cardCount)
					{
						cardList[i].gameObject.SetActive(true);
						cardList[i].transform.localPosition = cardPos[i];
						cardList[i].sizeDelta = new Vector2(setting.edgeLength, setting.edgeLength);
					} else
					{
						cardList[i].gameObject.SetActive(false);
					}
				}
				break;
			case UseLayout.Infinite:
				LoadCard(totalCardCount);
				Vector2[] cardPos2 = GetCardPosition(totalCardCount);
				for(int i = 0 ; i < cardList.Count ; ++i)
				{
					if(i < totalCardCount)
					{
						cardList[i].gameObject.SetActive(true);
						cardList[i].transform.localPosition = cardPos2[i];
						cardList[i].sizeDelta = new Vector2(cardSize, cardSize);
					} else
					{
						cardList[i].gameObject.SetActive(false);
					}
				}
				break;
		}
	}

	void LoadCard(int cardCount)
	{
		if(cardList.Count < cardCount)
		{
			GameObject cardPrefab = Resources.Load("Card/CardBase") as GameObject;
			for(int i = cardList.Count ; i < cardCount ; ++i)
			{
				GameObject tmp = Instantiate(cardPrefab) as GameObject;
				tmp.name = "NormalCard_" + i.ToString();
				tmp.transform.SetParent(cardParent);
				tmp.transform.localScale = Vector3.one;
				tmp.SetActive(false);
				cardList.Add(tmp.GetComponent<RectTransform>());
			}
		}
	}

	Vector2[] GetCardPosition(CardArraySetting setting)
	{
		Vector2[] cardPos = new Vector2[setting.row * setting.column];

		for(int i = 0 ; i < cardPos.Length ; ++i)
		{
			float x = setting.realFirstPosition.x + (i % setting.column) * (setting.edgeLength + setting.cardGap);
			float y = setting.realFirstPosition.y - (i / setting.column) * (setting.edgeLength + setting.cardGap);
			cardPos[i] = new Vector2(x, y);
		}

		return cardPos;
	}

	Vector2[] GetCardPosition(int cardCount)
	{
		switch(cardCount)
		{
			case 6:
				GameSettingManager.currentLevel = LevelDifficulty.EASY;
				return GetCardPosition(GameSettingManager.GetCurrentCardArraySetting());
			case 12:
				GameSettingManager.currentLevel = LevelDifficulty.NORMAL;
				return GetCardPosition(GameSettingManager.GetCurrentCardArraySetting());
			case 20:
				GameSettingManager.currentLevel = LevelDifficulty.HARD;
				return GetCardPosition(GameSettingManager.GetCurrentCardArraySetting());
			case 30:
				GameSettingManager.currentLevel = LevelDifficulty.CRAZY;
				return GetCardPosition(GameSettingManager.GetCurrentCardArraySetting());
			default:
				return null;
		}
	}
}
