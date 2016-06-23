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
	
	public void LoadCard(BoolCardBase canFlipCardNow)
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
				pickCard.Init(canFlipCardNow, CardFlipFinish);
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

	protected override void CardFlipFinish(CardBase card)
	{
		bool match = card.GetCardId() == targetCard.GetCardId();

		if(match)
		{
			AudioManager.Instance.PlayOneShot("GamePlayGetPair");
			cardsOnTable.Remove(card);
			card.Match();
		}
		else
		{
			card.MisMatch();
		}

		cardMatch(match, card);
	}

	public void SetUsingCard(int cardCount)
	{
		if(cardCount > normalCardDeck.Count)
		{
			Debug.LogError("Normal Card Deck have no enough cards.");
			return;
		}
		
		usingCardDeck.Clear();

		for(int i = 0 ; i < cardCount ; ++i)
			usingCardDeck.Add(normalCardDeck[i]);
	}

	public void FlipAllCard(bool flipTargetCard)
	{
		if(flipTargetCard)
			targetCard.FlipBySystem();

		foreach(CardBase card in cardsOnTable)
			card.FlipBySystem();
	}

	public void ClearAllCard()
	{
		targetCard.Match();

		foreach(CardBase card in cardsOnTable)
			card.Match();
	}
	
	public IEnumerator DealCard(float cardSize, Vector2[] cardPos, int targetCardCount)
	{
		this.targetCardCount = targetCardCount;

		ShuffleCardDeck();
		targetCard.DealCard(Vector2.zero, shiftAmount, 0f, appearDuration, 200f);
		float delayDuration = dealTime / usingCardDeck.Count;
		cardsOnTable.Clear();
        for(int i = 0 ; i < usingCardDeck.Count ; ++i)
		{
			cardsOnTable.Add(usingCardDeck[i]);
			usingCardDeck[i].DealCard(cardPos[i], shiftAmount, delayDuration * i, appearDuration, cardSize);
		}
		yield return new WaitForSeconds(dealTime);
	}

	protected override void ShuffleCardDeck()
	{
		Sprite[] thisTimeCardImage = GetCardImage(usingCardDeck.Count, targetCardCount);

		for(int i = 0 ; i < usingCardDeck.Count ; ++i)
		{
			usingCardDeck[i].SetSprite(thisTimeCardImage[i], CardState.Back);
			usingCardDeck[i].SetCardId(thisTimeCardImage[i].name);
		}

		if(usingCardDeck.Count > 2)
		{
			for(int i = 0 ; i < usingCardDeck.Count ; ++i)
			{
				int randomIndex = Random.Range(0, usingCardDeck.Count);
				while(randomIndex == i)
					randomIndex = Random.Range(0, usingCardDeck.Count);

				CardBase tmp = usingCardDeck[i];
				usingCardDeck[i] = usingCardDeck[randomIndex];
				usingCardDeck[randomIndex] = tmp;
			}
		}else if(usingCardDeck.Count == 2)
		{
			bool swap = Random.Range(0, 2) == 0;
			if(swap)
			{
				CardBase tmp = usingCardDeck[0];
				usingCardDeck[0] = usingCardDeck[1];
				usingCardDeck[1] = tmp;
			}
		}
	}

	Sprite[] GetCardImage(int cardCount, int targetCardCount)
	{
		Sprite[] choosenCardImage = new Sprite[cardCount];

		List<Sprite> unchooseCardImage = new List<Sprite>(cardImage);

		int targetIndex = Random.Range(0, unchooseCardImage.Count);
		Sprite targetCardSprite = unchooseCardImage[targetIndex];
		unchooseCardImage.RemoveAt(targetIndex);

		targetCard.SetSprite(targetCardSprite, CardState.Back);
		targetCard.SetCardId(targetCardSprite.name);
		
		for(int i = 0 ; i < choosenCardImage.Length ; ++i)
		{
			if(i < targetCardCount)
			{
				choosenCardImage[i] = targetCardSprite;
			} else
			{
				choosenCardImage[i] = unchooseCardImage[Random.Range(0, unchooseCardImage.Count)];
			}
		}

		return choosenCardImage;
	}
}
