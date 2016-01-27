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
	Vector2[] pos;
	Vector2 shiftAmount = new Vector2(-100, 50);
	float appearDuration = 0.3f;
	float dealTime = 0.5f;
	CardBase[] cardsDeck;
	Queue<CardBase> waitForCompare = new Queue<CardBase>();
	Queue<CardBase> waitForMatch = new Queue<CardBase>();
	List<CardBase> cardsOnTable = new List<CardBase>();
	int goldCardCount = 0;
	bool lockFlipCard = false;

	public override IEnumerator Init()
	{
		ToggleMask(true);
		getLuckyEffect.SetActive(false);

		image_Theme.sprite = InventoryManager.Instance.GetCurrentThemeSprite();
		CardArraySetting setting = GameSettingManager.GetCurrentCardArraySetting();
		cardsDeck = new CardBase[setting.column * setting.row];
		pos = new Vector2[cardsDeck.Length];

		GameObject cardPrefab = Resources.Load("Card/CardBase") as GameObject;
		for(int i = 0 ; i < cardsDeck.Length ; ++i)
		{
			GameObject tmp = Instantiate(cardPrefab) as GameObject;
			tmp.name = cardPrefab.name + i.ToString();
			tmp.transform.SetParent(cardParent);
			tmp.transform.localScale = Vector3.one;
			tmp.SetActive(false);
			cardsDeck[i] = tmp.AddComponent<CardBase>();
			cardsDeck[i].Init(CanFlipCardNow, CardFlipFinish, setting.edgeLength);
			float x = setting.realFirstPosition.x + (i % setting.column) * (setting.edgeLength + setting.cardGap);
			float y = setting.realFirstPosition.y - (i / setting.column) * (setting.edgeLength + setting.cardGap);
			pos[i] = new Vector2(x, y);
		}
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
		cardImage = new Sprite[30];
		for(int i = 0 ; i < 30 ; ++i)
		{
			ResourceRequest request = Resources.LoadAsync<Sprite>(string.Format("CardImage/CardImage_{0}", i.ToString("D3")));
			yield return request;
			cardImage[i] = (Sprite)request.asset;
		}
	}

	public IEnumerator DealCard(int unknownCardCount = 0)
	{
		ShuffleCardDeck();
		float delayDuration = dealTime / cardsDeck.Length;		
		UpdateGoldCard();
		for(int i = 0 ; i < cardsDeck.Length ; ++i)
		{
			cardsOnTable.Add(cardsDeck[i]);
            cardsDeck[i].DealCard(pos[i], shiftAmount, delayDuration * i, appearDuration);
		}
		yield return new WaitForSeconds(dealTime);
	}

	public void FlipAllCard()
	{
		foreach(CardBase card in cardsDeck)
			card.FlipBySystem();
	}
	
	public void ToggleCardGlow(bool turnOn)
	{
		foreach(CardBase card in cardsDeck)
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
			foreach(CardBase card in cardsDeck)
				card.ToggleGoldCard(true);
		}
		else if(goldCardCount == 0)
		{
			foreach(CardBase card in cardsDeck)
				card.ToggleGoldCard(false);
		}
		else
		{
			foreach(CardBase card in cardsDeck)
				card.ToggleGoldCard(false);

			List<int> tickets = new List<int>();
			for(int i = 0 ; i < cardsDeck.Length ; ++i)
				tickets.Add(i);

			for(int i = 0 ; i < goldCardCount ; ++i)
			{
				int randomIndex = Random.Range(0, tickets.Count);
				tickets.RemoveAt(randomIndex);
				cardsDeck[randomIndex].ToggleGoldCard(true);
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
		Sprite[] thisTimeCardImage = GetCardImage(cardsDeck.Length);
		for(int i = 0 ; i < cardsDeck.Length ; ++i)
		{
			cardsDeck[i].SetSprite(thisTimeCardImage[i], CardState.Back);
			cardsDeck[i].SetCardId(thisTimeCardImage[i].name);
		}

		for(int i = 0 ; i < cardsDeck.Length ; ++i)
		{
			int randomIndex = Random.Range(0, cardsDeck.Length);
			while(randomIndex == i)
				randomIndex = Random.Range(0, cardsDeck.Length);

			CardBase tmp = cardsDeck[i];
			cardsDeck[i] = cardsDeck[randomIndex];
			cardsDeck[randomIndex] = tmp;
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
