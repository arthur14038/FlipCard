using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class PickGameMainView : GameMainView {
	[SerializeField]
	CanvasGroup group_PickCard;
	[SerializeField]
	Text text_PickTitle;
	[SerializeField]
	RectTransform group_Hint;
	Image[] hints;
	PickCard targetCard;
	int targetCardCount;

	public override IEnumerator Init()
	{
		hints = group_Hint.GetComponentsInChildren<Image>(true);

		yield return StartCoroutine(base.Init());
	}
	
	public void LoadCard()
	{
		GameObject cardPrefab = Resources.Load("Card/CardBase") as GameObject;

		for(int i = 0 ; i < 21 ; ++i)
		{
			GameObject tmp = Instantiate(cardPrefab) as GameObject;
			if(i != 20)
			{
				tmp.name = "NormalCard_" + i.ToString();
				tmp.transform.SetParent(cardParent);
				tmp.transform.localScale = Vector3.one;
				tmp.SetActive(false);
				PickCard pickCard = tmp.AddComponent<PickCard>();
				pickCard.Init(CanFlipCardNow, CardFlipFinish);
				normalCardDeck.Add(pickCard);
			}else
			{
				tmp.name = "TargetCard";
				tmp.transform.SetParent(group_PickCard.transform);
				tmp.transform.localScale = Vector3.one;
				tmp.transform.localPosition = Vector3.zero;
				tmp.SetActive(false);
				targetCard = tmp.AddComponent<PickCard>();
				targetCard.Init(null, null);
				targetCard.SetDisable();
            }
		}
	}

	public void SetUsingCard(int cardCount)
	{
		usingCardDeck.Clear();

		for(int i = 0 ; i < cardCount ; ++i)
			usingCardDeck.Add(normalCardDeck[i]);
	}

	public IEnumerator DealCard(float cardSize, Vector2[] cardPos, int targetCardCount)
	{
		this.targetCardCount = targetCardCount;
		Debug.LogFormat("DealCard cardSize: {0}, targetCardCount: {1}", cardSize, targetCardCount);

		ShuffleCardDeck();
		targetCard.DealCard(Vector2.zero, shiftAmount, 0f, appearDuration, 200f);
		float delayDuration = dealTime / usingCardDeck.Count;
		for(int i = 0 ; i < usingCardDeck.Count ; ++i)
		{
			cardsOnTable.Add(usingCardDeck[i]);
			usingCardDeck[i].DealCard(cardPos[i], shiftAmount, delayDuration * i, appearDuration, cardSize);
		}
		yield return new WaitForSeconds(dealTime);
	}

	protected override void ShuffleCardDeck()
	{
		Debug.LogFormat("ShuffleCardDeck usingCardDeck.Count: {0}", usingCardDeck.Count);
		Sprite[] thisTimeCardImage = GetCardImage(usingCardDeck.Count, targetCardCount);

		for(int i = 0 ; i < usingCardDeck.Count ; ++i)
		{
			usingCardDeck[i].SetSprite(thisTimeCardImage[i], CardState.Back);
			usingCardDeck[i].SetCardId(thisTimeCardImage[i].name);
		}

		for(int i = 0 ; i < usingCardDeck.Count ; ++i)
		{
			int randomIndex = Random.Range(0, usingCardDeck.Count);
			while(randomIndex == i)
				randomIndex = Random.Range(0, usingCardDeck.Count);

			CardBase tmp = normalCardDeck[i];
			usingCardDeck[i] = usingCardDeck[randomIndex];
			usingCardDeck[randomIndex] = tmp;
		}
	}

	Sprite[] GetCardImage(int cardCount, int targetCardCount)
	{
		Debug.LogFormat("GetCardImage cardCount: {0}, targetCardCount: {1}", cardCount, targetCardCount);
		Sprite[] chooseCardImage = new Sprite[cardCount];

		List<Sprite> nonetargetCardImage = new List<Sprite>(cardImage);
		int targetIndex = Random.Range(0, nonetargetCardImage.Count);
		Sprite targetCardSprite = nonetargetCardImage[targetIndex];
		nonetargetCardImage.RemoveAt(targetIndex);

		targetCard.SetSprite(targetCardSprite, CardState.Back);
		targetCard.SetCardId(targetCardSprite.name);

		for(int i = 0 ; i < cardCount ; ++i)
		{
			if(i < targetCardCount)
			{
				chooseCardImage[i] = targetCardSprite;
            } else
			{
				chooseCardImage[i] = nonetargetCardImage[Random.Range(0, nonetargetCardImage.Count)];
            }
		}

		return chooseCardImage;
	}
}
