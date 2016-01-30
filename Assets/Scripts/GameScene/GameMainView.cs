using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameMainView : AbstractView
{
	public Transform scoreTextParent;
	public Transform cardParent;
	public GameObject image_Mask;
	public GameObject scoreTextPrefab;
	public GameObject getLuckyEffect;
	public Image image_Theme;
	public VoidNoneParameter completeOneRound;
	public VoidBoolAndCards cardMatch;
	Sprite[] cardImage;
	Queue<ScoreText> scoreTextQueue = new Queue<ScoreText>();
	Vector2 shiftAmount = new Vector2(-100, 50);
	float appearDuration = 0.3f;
	float dealTime = 0.5f;
	Queue<CardBase> waitForCompare = new Queue<CardBase>();
	Queue<CardBase> waitForMatch = new Queue<CardBase>();
	List<CardBase> usingCardDeck = new List<CardBase>();
	List<CardBase> cardsOnTable = new List<CardBase>();
	List<CardBase> normalCardDeck = new List<CardBase>();
	List<UnknownCard> unknownCardDeck = new List<UnknownCard>();
	int goldCardCount = 0;
	bool lockFlipCard = false;

	public override IEnumerator Init()
	{
		ToggleMask(true);
		getLuckyEffect.SetActive(false);

		image_Theme.sprite = InventoryManager.Instance.GetCurrentThemeSprite();
		
		for(int i = 0 ; i < 8 ; ++i)
		{
			GameObject tmp = Instantiate(scoreTextPrefab) as GameObject;
			tmp.name = scoreTextPrefab.name;
			tmp.transform.SetParent(scoreTextParent);
			tmp.transform.localScale = Vector3.one;
			ScoreText st = tmp.GetComponent<ScoreText>();
			st.Init(SaveScoreText);
			SaveScoreText(st);
		}
		scoreTextPrefab = null;
		cardImage = new Sprite[30];
		for(int i = 0 ; i < 30 ; ++i)
		{
			ResourceRequest request = Resources.LoadAsync<Sprite>(string.Format("CardImage/CardImage_{0}", i.ToString("D3")));
			yield return request;
			cardImage[i] = (Sprite)request.asset;
		}
	}
	
	public void LoadCard(int normalCardCount, int unknownCardCount)
	{
		GameObject cardPrefab = Resources.Load("Card/CardBase") as GameObject;
		for(int i = 0 ; i < normalCardCount ; ++i)
		{
			GameObject tmp = Instantiate(cardPrefab) as GameObject;
			tmp.name = "NormalCard_" + i.ToString();
			tmp.transform.SetParent(cardParent);
			tmp.transform.localScale = Vector3.one;
			tmp.SetActive(false);
			CardBase normalCard = tmp.AddComponent<CardBase>();
			normalCard.Init(CanFlipCardNow, CardFlipFinish);
			normalCardDeck.Add(normalCard);
		}

		for(int i = 0 ; i < unknownCardCount ; ++i)
		{
			GameObject tmp = Instantiate(cardPrefab) as GameObject;
			tmp.name = "UnknownCard_" + i.ToString();
			tmp.transform.SetParent(cardParent);
			tmp.transform.localScale = Vector3.one;
			tmp.SetActive(false);
			UnknownCard unknownCard = tmp.AddComponent<UnknownCard>();
			unknownCard.Init(CanFlipCardNow, CardFlipFinish);
			unknownCardDeck.Add(unknownCard);
		}
	}

	public void SetUsingCard(int normalCardCount, int unknownCardCount)
	{
		if(normalCardCount > normalCardDeck.Count)
		{
			Debug.LogError("Normal Card Deck have no enough cards.");
			return;
		}

		if(unknownCardCount > unknownCardDeck.Count)
		{
			Debug.LogError("Unknown Card Deck have no enough cards.");
			return;
		}

		usingCardDeck.Clear();
		for(int i = 0 ; i < normalCardCount ; ++i)
			usingCardDeck.Add(normalCardDeck[i]);
		for(int i = 0 ; i < unknownCardCount ; ++i)
			usingCardDeck.Add(unknownCardDeck[i]);
	}

	public IEnumerator DealCard(float cardSize, Vector2[] cardPos)
	{
		ShuffleCardDeck();
		float delayDuration = dealTime / usingCardDeck.Count;		
		UpdateGoldCard();
		for(int i = 0 ; i < usingCardDeck.Count ; ++i)
		{
			cardsOnTable.Add(usingCardDeck[i]);
			usingCardDeck[i].DealCard(cardPos[i], shiftAmount, delayDuration * i, appearDuration, cardSize);
		}
		yield return new WaitForSeconds(dealTime);
	}

	public void FlipAllCard()
	{
		foreach(CardBase card in cardsOnTable)
			card.FlipBySystem();
	}
	
	public void ToggleCardGlow(bool turnOn)
	{
		foreach(CardBase card in usingCardDeck)
			card.ToggleCardGlow(turnOn);
	}

	public void SetGoldCard(int count, bool setImmediate = true)
	{
		goldCardCount = count;
		if(setImmediate)
			UpdateGoldCard();
	}

	void UpdateGoldCard()
	{
		if(goldCardCount < 0)
		{
			foreach(CardBase card in usingCardDeck)
				card.ToggleGoldCard(true);
		}
		else if(goldCardCount == 0)
		{
			foreach(CardBase card in usingCardDeck)
				card.ToggleGoldCard(false);
		}
		else
		{
			foreach(CardBase card in usingCardDeck)
				card.ToggleGoldCard(false);

			List<int> tickets = new List<int>();
			for(int i = 0 ; i < usingCardDeck.Count ; ++i)
				tickets.Add(i);

			for(int i = 0 ; i < goldCardCount ; ++i)
			{
				int randomIndex = Random.Range(0, tickets.Count);
				usingCardDeck[tickets[randomIndex]].ToggleGoldCard(true);
				tickets.RemoveAt(randomIndex);
			}
		}
	}

	bool CanFlipCardNow(CardBase card)
	{
		if(lockFlipCard)
			return false;

		waitForCompare.Enqueue(card);
		if(waitForCompare.Count > 1)
		{
			CardBase cardA = waitForCompare.Dequeue();
			CardBase cardB = waitForCompare.Dequeue();
			if(cardA.GetCardId() != cardB.GetCardId())
			{
				if(GameSettingManager.currentMode == GameMode.Competition)
					lockFlipCard = true;
			}
		}
		return true;
	}

	void CardFlipFinish(CardBase card)
	{
		waitForMatch.Enqueue(card);
		if(waitForMatch.Count > 1)
		{
			CardBase cardA = waitForMatch.Dequeue();
			CardBase cardB = waitForMatch.Dequeue();
			bool isMatch = false;
			if(cardA.GetCardId() == cardB.GetCardId())
			{
				if(cardA.IsGoldCard() || cardB.IsGoldCard())
				{
					getLuckyEffect.SetActive(false);
					getLuckyEffect.SetActive(true);
				}
				AudioManager.Instance.PlayOneShot("GamePlayGetPair");
				isMatch = true;
				cardA.Match();
				cardB.Match();
				cardsOnTable.Remove(cardA);
				cardsOnTable.Remove(cardB);
			}
			else
			{
				lockFlipCard = false;
				cardA.MisMatch();
				cardB.MisMatch();
			}
			if(cardMatch != null)
				cardMatch(isMatch, cardA, cardB);
		}
		if(cardsOnTable.Count == 0 && completeOneRound != null)
			completeOneRound();
	}
	
	public void ToggleMask(bool value)
	{
		if(image_Mask.activeSelf != value)
			image_Mask.SetActive(value);
	}

	public void ShowScoreText(int score, Vector2 pos)
	{
		ScoreText st = scoreTextQueue.Dequeue();
		st.ShowScoreText(score, pos);
	}

	void SaveScoreText(ScoreText st)
	{
		scoreTextQueue.Enqueue(st);
	}
	
	void ShuffleCardDeck()
	{
		Sprite[] thisTimeCardImage = GetCardImage(usingCardDeck.Count);
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

			CardBase tmp = usingCardDeck[i];
			usingCardDeck[i] = usingCardDeck[randomIndex];
			usingCardDeck[randomIndex] = tmp;
		}
	}

	Sprite[] GetCardImage(int count)
	{
		Sprite[] choosenCardFace = new Sprite[count];
		if(cardImage.Length > count / 2)
		{
			int[] chooseCardIndex = new int[count / 2];
			List<int> tickets = new List<int>();
			for(int i = 0 ; i < cardImage.Length ; ++i)
				tickets.Add(i);

			for(int i = 0 ; i < chooseCardIndex.Length ; ++i)
			{
				int selectIndex = Random.Range(0, tickets.Count);
				chooseCardIndex[i] = tickets[selectIndex];
				tickets.RemoveAt(selectIndex);
			}

			for(int i = 0 ; i < choosenCardFace.Length ; ++i)
			{
				choosenCardFace[i] = cardImage[chooseCardIndex[i / 2]];
			}
		} else if(cardImage.Length == count / 2)
		{
			for(int i = 0 ; i < count ; ++i)
			{
				choosenCardFace[i] = cardImage[i / 2];
			}
		} else
		{
			Debug.LogError("CardFace is not enough");
		}

		return choosenCardFace;
	}
	
	protected override IEnumerator HideUIAnimation()
	{
		yield return null;
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		yield return null;
		showCoroutine = null;
	}
}
