using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TestCardArray))]
public class TestCardArrayEditor : Editor
{
	public override void OnInspectorGUI()
	{
		TestCardArray myTarget = (TestCardArray)target;

		myTarget.cardParent = EditorGUILayout.ObjectField("Card Parent", myTarget.cardParent, typeof(Transform), true) as Transform;
		myTarget.useLayout = (UseLayout)EditorGUILayout.EnumPopup("Use Layout", myTarget.useLayout);
		switch(myTarget.useLayout)
		{
			case UseLayout.LevelDifficulty:
				myTarget.difficulty = (LevelDifficulty)EditorGUILayout.EnumPopup("Difficulty", myTarget.difficulty);
				break;
			case UseLayout.Infinite:
				break;
		}
		if(GUILayout.Button("Show Cards"))
		{
			myTarget.LoadAndShowCards();
		}
		//EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
	}
}

public enum UseLayout {LevelDifficulty, Infinite}
public class TestCardArray : MonoBehaviour {
	public Transform cardParent;
	public UseLayout useLayout = UseLayout.LevelDifficulty;
	public LevelDifficulty difficulty;
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
}
