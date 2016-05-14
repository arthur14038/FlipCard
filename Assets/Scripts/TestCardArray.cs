using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCardArray : MonoBehaviour {
	public Transform cardParent;
	public int totalCardCount = 6;
	public float cardSize = 192f;
	public float gap = 50f;
	public Vector2 center = Vector2.zero;
	public GameMode currentMode = GameMode.FlipCard;
	List<RectTransform> cardList = new List<RectTransform>();
	
	public void LoadAndShowCards()
	{
		if(currentMode != GameMode.PickCard)
			GameSettingManager.LoadData();

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
	}

	public void PrintPos()
	{
		string positionInfo = "";
		for(int i = 0 ; i < cardList.Count ; ++i)
		{
			if(cardList[i].gameObject.activeInHierarchy)
			{
				positionInfo += string.Format("{0},{1}", cardList[i].anchoredPosition.x, cardList[i].anchoredPosition.y);
				if(i != cardList.Count - 1)
					positionInfo += ";";
			}
		}
		Debug.Log(positionInfo);
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
	
	Vector2[] GetCardPosition(int cardCount)
	{
		if(currentMode != GameMode.PickCard)
			return GameSettingManager.GetFlipCardArraySetting(cardCount).realCardPosition;
		else
		{
			Vector2[] cardPos = new Vector2[cardCount];
            switch(cardCount)
			{
				case 1:
					return new Vector2[] {new Vector2(0f, -350f) };
				case 2:
					return new Vector2[] { new Vector2(-150f, -350f), new Vector2(150f, -350f) };
				case 3:
					return new Vector2[] { new Vector2(0f, -200f), new Vector2(-150f, -500f), new Vector2(150f, -500f) };
				case 4:
					return new Vector2[] { new Vector2(-150f, -200f), new Vector2(150f, -200f), new Vector2(-150f, -500f), new Vector2(150f, -500f) };
				case 5:
					return new Vector2[] { new Vector2(-300f, -200f), new Vector2(0f, -200f), new Vector2(300f, -200f), new Vector2(-150f, -500f), new Vector2(150f, -500f) };
				case 6:
					for(int i = 0 ; i < cardCount ; ++i)
					{
						float x = -300 + 300 * (i % 3);
						float y = -200 + -300 * (i / 3);
						cardPos[i] = new Vector2(x, y);
					}
					break;
				case 7:
				case 8:
				case 9:
					for(int i = 0 ; i < cardCount ; ++i)
					{
						float x = -250 + 250 * (i % 3);
						float y = -100 + -250 * (i / 3);
						cardPos[i] = new Vector2(x, y);
					}
					break;
				case 10:
				case 11:
				case 12:
					List<Vector2> pos = new List<Vector2>();
					for(int i = 0 ; i < 12 ; ++i)
					{
						float x = -375 + 250 * (i % 4);
						float y = -100 + -250 * (i / 4);
						pos.Add(new Vector2(x, y));
					}
					cardPos = pos.ToArray();
					break;
				case 15:
					for(int i = 0 ; i < cardCount ; ++i)
					{
						float x = -400 + 200 * (i % 5);
						float y = -150 + -200 * (i / 5);
						cardPos[i] = new Vector2(x, y);
					}
					break;
				case 16:
					for(int i = 0 ; i < cardCount ; ++i)
					{
						float x = -300 + 200 * (i % 4);
						float y = -50 + -200 * (i / 4);
						cardPos[i] = new Vector2(x, y);
					}
					break;
				case 20:
					for(int i = 0 ; i < cardCount ; ++i)
					{
						float x = -400 + 200 * (i % 5);
						float y = -50 + -200 * (i / 5);
						cardPos[i] = new Vector2(x, y);
					}
					break;
			}
			return cardPos;
		}
	}
}
