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
	[SerializeField]
	RectTransform group_Perfect;
	Image[] hints;
	PickCard targetCard;
	int targetCardCount;
	Color hintActiveColor = new Color(2/255f, 1f, 99/255f);

	public override IEnumerator Init()
	{
		hints = group_Hint.GetComponentsInChildren<Image>(true);
		group_Perfect.gameObject.SetActive(false);

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

	public IEnumerator PerfectEffect()
	{
		group_Perfect.gameObject.SetActive(true);

		group_Perfect.localScale = Vector3.zero;
		yield return group_Perfect.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
		yield return new WaitForSeconds(0.4f);
		yield return group_Perfect.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();

		group_Perfect.gameObject.SetActive(false);
	}

	public void SetHint(int totalCount, int activeCount)
	{
		if(totalCount > -1 && totalCount < hints.Length)
		{
			for(int i = 0 ; i < hints.Length ; ++i)
			{
				if(i < totalCount)
				{
					if(!hints[i].gameObject.activeSelf)
						hints[i].gameObject.SetActive(true);

					if(i < activeCount)
						hints[i].color = hintActiveColor;
					else
						hints[i].color = Color.gray;
				} else
				{
					if(hints[i].gameObject.activeSelf)
						hints[i].gameObject.SetActive(false);
				}
			}
		}
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

		bool getHard = false;
		int dice = Random.Range(0, 8);
        if(targetCardCount > 4)
			getHard = dice > 5;

		int chooseCardGroup = Random.Range(0, GameSettingManager.totalImageGroupCount);
		if(getHard)
		{
			if(dice == 6)
				chooseCardGroup = 0;
			if(dice == 7)
				chooseCardGroup = 7;
		}
		CardImageGroup group = GameSettingManager.GetCardImageGroup(chooseCardGroup);
		List<string> unchooseCardImage = new List<string>(group.imageNames);

		int targetIndex = Random.Range(0, unchooseCardImage.Count);
		Sprite targetCardSprite = cardImageDictionary[unchooseCardImage[targetIndex]];
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
				choosenCardImage[i] = cardImageDictionary[unchooseCardImage[Random.Range(0, unchooseCardImage.Count)]];
			}
		}

		return choosenCardImage;
	}
}
