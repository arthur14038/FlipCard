using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
				string positionInfo = "";
				for(int i = 0 ; i < cardPos2.Length ; ++i)
				{
					positionInfo += string.Format("{0},{1}", cardPos2[i].x, cardPos2[i].y);
					if(i != cardPos2.Length - 1)
						positionInfo += ";";
				}
				Debug.Log(positionInfo);
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
			case 8:
				GameSettingManager.currentLevel = LevelDifficulty.NORMAL;
				Vector2[] pos = GetCardPosition(GameSettingManager.GetCurrentCardArraySetting());
				List<Vector2> tmpPos = new List<Vector2>(pos);
				tmpPos.RemoveAt(11);
				tmpPos.RemoveAt(9);
				tmpPos.RemoveAt(2);
				tmpPos.RemoveAt(0);
				return tmpPos.ToArray();
            case 12:
				GameSettingManager.currentLevel = LevelDifficulty.NORMAL;
				return GetCardPosition(GameSettingManager.GetCurrentCardArraySetting());
			case 16:
				GameSettingManager.currentLevel = LevelDifficulty.HARD;
				Vector2[] pos1 = GetCardPosition(GameSettingManager.GetCurrentCardArraySetting());
				List<Vector2> tmpPos2 = new List<Vector2>(pos1);
				tmpPos2.RemoveAt(19);
				tmpPos2.RemoveAt(18);
				tmpPos2.RemoveAt(17);
				tmpPos2.RemoveAt(16);
				for(int i = 0 ; i < tmpPos2.Count ; ++i)
				{
					Vector2 tmpVec2 = tmpPos2[i];
                    tmpVec2.y -= 130f;
					tmpPos2[i] = tmpVec2;
                }
				return tmpPos2.ToArray();
			case 20:
				GameSettingManager.currentLevel = LevelDifficulty.HARD;
				return GetCardPosition(GameSettingManager.GetCurrentCardArraySetting());
			case 24:
				GameSettingManager.currentLevel = LevelDifficulty.CRAZY;
				Vector2[] pos2 = GetCardPosition(GameSettingManager.GetCurrentCardArraySetting());
				List<Vector2> tmpPos3 = new List<Vector2>(pos2);
				tmpPos3.RemoveAt(29);
				tmpPos3.RemoveAt(24);
				tmpPos3.RemoveAt(19);
				tmpPos3.RemoveAt(14);
				tmpPos3.RemoveAt(9);
				tmpPos3.RemoveAt(4);
				for(int i = 0 ; i < tmpPos3.Count ; ++i)
				{
					Vector2 tmpVec2 = tmpPos3[i];
					tmpVec2.x += 100f;
					tmpPos3[i] = tmpVec2;
				}
				return tmpPos3.ToArray();
			case 30:
				GameSettingManager.currentLevel = LevelDifficulty.CRAZY;
				return GetCardPosition(GameSettingManager.GetCurrentCardArraySetting());
			default:
				return null;
		}
	}
}
