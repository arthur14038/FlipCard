using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCardArray : MonoBehaviour {
	public Transform cardParent;
	public int totalCardCount = 6;
	public float cardSize = 192f;
	public float gap = 50f;
	public Vector2 center = Vector2.zero;
	List<RectTransform> cardList = new List<RectTransform>();
	
	public void LoadAndShowCards()
	{
		GameSettingManager.LoadData();
		{
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
	
	Vector2[] GetCardPosition(int cardCount)
	{
		return GameSettingManager.GetFlipCardArraySetting(cardCount).realCardPosition;
	}
}
