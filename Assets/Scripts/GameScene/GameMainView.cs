using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMainView : AbstractView
{
	public Transform scoreTextParent;
	public Transform cardParent;
	public GameObject image_Mask;
	public GameObject scoreTextPrefab;
	public GameObject cardPrefab;
	public GameObject getLuckyEffect;
	public Sprite[] cardBack;
	public Sprite[] cardFace;
	public Sprite[] cardImage;
	public Sprite unknownCardImageSprite;
	public VoidNoneParameter completeOneRound;
	public VoidBoolAndCards cardMatch;
	Queue<ScoreText> scoreTextQueue = new Queue<ScoreText>();
	Vector2[] pos;
	Vector2 shiftAmount = new Vector2(-100, 50);
	float appearDuration = 0.3f;
	float dealTime = 0.5f;
	Card[] cards;
	Queue<Card> waitForCompare = new Queue<Card>();
	Queue<Card> waitForMatch = new Queue<Card>();
	List<Card> cardsOnTable = new List<Card>();
	int unknownCard1 = 0;
	int unknownCard2 = 0;
	int goldCardCount = 0;
	bool lockFlipCard = false;

	public override IEnumerator Init()
	{
		ToggleMask(true);
		getLuckyEffect.SetActive(false);

		CardArraySetting setting = GameSettingManager.GetCurrentCardArraySetting();
        cards = new Card[setting.column * setting.row];
		pos = new Vector2[cards.Length];

		for(int i = 0 ; i < cards.Length ; ++i)
		{
			GameObject tmp = Instantiate(cardPrefab) as GameObject;
			tmp.name = cardPrefab.name + i.ToString();
			tmp.transform.SetParent(cardParent);
			tmp.transform.localScale = Vector3.one;
			tmp.SetActive(false);
			cards[i] = tmp.GetComponent<Card>();
			cards[i].SetSize(setting.edgeLength);
			cards[i].Init(CanFlipCardNow, CardFlipFinish);
			float x = setting.realFirstPosition.x + (i % setting.column) * (setting.edgeLength + setting.cardGap);
			float y = setting.realFirstPosition.y - (i / setting.column) * (setting.edgeLength + setting.cardGap);
			pos[i] = new Vector2(x, y);
		}
		cardPrefab = null;
		yield return null;

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
		yield return null;
	}

	public IEnumerator DealCard(bool activeUnknownCard = false)
	{
		Sprite[] thisTimeCardImage = GetCardImage(cards.Length);
		float delayDuration = dealTime / cards.Length;

		if(activeUnknownCard)
		{
			unknownCard1 = Random.Range(0, thisTimeCardImage.Length);
			unknownCard2 = Random.Range(0, thisTimeCardImage.Length);
			while(thisTimeCardImage[unknownCard2].name == thisTimeCardImage[unknownCard1].name)
				unknownCard2 = Random.Range(0, thisTimeCardImage.Length);
		}

		UpdateCardType();
		for(int i = 0 ; i < cards.Length ; ++i)
		{
			cardsOnTable.Add(cards[i]);
			cards[i].SetCard(cardBack[0], cardFace[0], thisTimeCardImage[i], Card.CardState.Back);

			if(activeUnknownCard)
			{
				if(i == unknownCard1 || i == unknownCard2)
					cards[i].SetCardImage(unknownCardImageSprite);
			}

			cards[i].Appear(pos[i], shiftAmount, delayDuration * i, appearDuration);
		}
		yield return new WaitForSeconds(dealTime);
	}

	public void FlipAllCard()
	{
		foreach(Card card in cards)
			card.Flip();
	}

	public void ResetUnknownCard()
	{
		cards[unknownCard1].SetCardImageToOriginal();
		cards[unknownCard2].SetCardImageToOriginal();
	}

	public void SetGoldCard(int count, bool setImmediate = true)
	{
		goldCardCount = count;
		if(setImmediate)
			UpdateCardType();
	}

	public void ToggleCardGlow(bool turnOn)
	{
		foreach(Card card in cards)
			card.ToggleCardGlow(turnOn);
	}
		
	void UpdateCardType()
	{
		if(goldCardCount < 0)
		{
			foreach(Card card in cards)
				card.SetCardType(Card.CardType.Gold);
		}
		else if(goldCardCount == 0)
		{
			foreach(Card card in cards)
				card.SetCardType(Card.CardType.Normal);
		}
		else
		{
			if(goldCardCount >= cards.Length)
			{
				foreach(Card card in cardsOnTable)
					card.SetCardType(Card.CardType.Gold);
			}
			else
			{
				List<int> tickets = new List<int>();
				for(int i = 0 ; i < cards.Length ; ++i)
					tickets.Add(i);

				for(int i = 0 ; i < goldCardCount ; ++i)
				{
					int randomIndex = Random.Range(0, tickets.Count);
					tickets.RemoveAt(randomIndex);
					cards[randomIndex].SetCardType(Card.CardType.Gold);
				}
			}
		}
	}

	bool CanFlipCardNow(Card card)
	{
		if(lockFlipCard)
			return false;

		waitForCompare.Enqueue(card);
		if(waitForCompare.Count > 1)
		{
			Card cardA = waitForCompare.Dequeue();
			Card cardB = waitForCompare.Dequeue();
			if(cardA.GetCardId() != cardB.GetCardId())
			{
				if(GameSettingManager.currentMode == GameMode.Competition)
					lockFlipCard = true;
			}
		}
		return true;
	}

	void CardFlipFinish(Card card)
	{
		waitForMatch.Enqueue(card);
		if(waitForMatch.Count > 1)
		{
			Card cardA = waitForMatch.Dequeue();
			Card cardB = waitForMatch.Dequeue();
			bool isMatch = false;
			if(cardA.GetCardId() == cardB.GetCardId())
			{
				if(cardA.GetCardType() == Card.CardType.Gold || cardB.GetCardType() == Card.CardType.Gold)
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

		for(int i = 0 ; i < choosenCardFace.Length ; ++i)
		{
			int randomIndex = Random.Range(0, choosenCardFace.Length);
			while(randomIndex == i)
				randomIndex = Random.Range(0, choosenCardFace.Length);

			Sprite tmp = choosenCardFace[i];
			choosenCardFace[i] = choosenCardFace[randomIndex];
			choosenCardFace[randomIndex] = tmp;
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
