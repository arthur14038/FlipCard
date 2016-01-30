using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestCardArray : MonoBehaviour {
	public Transform cardParent;
	public int CardCount = 6;
	List<GameObject> cardList = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(10, 10, 150, 50), "Load"))
		{
			if(cardList.Count < CardCount)
			{
				GameObject cardPrefab = Resources.Load("Card/CardBase") as GameObject;
				for(int i = cardList.Count ; i < CardCount ; ++i)
				{
					GameObject tmp = Instantiate(cardPrefab) as GameObject;
					tmp.name = "NormalCard_" + i.ToString();
					tmp.transform.SetParent(cardParent);
					tmp.transform.localScale = Vector3.one;
					tmp.SetActive(false);
					cardList.Add(tmp);
				}
			}
		}

		if(GUI.Button(new Rect(200, 10, 150, 50), "Deal"))
		{
		}
	}
}
