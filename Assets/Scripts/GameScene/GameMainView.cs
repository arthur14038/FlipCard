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
	public Sprite[] cardBack;
	public Sprite[] cardFace;
	public Sprite[] cardImage;
	public VoidNoneParameter completeOneRound;
	public VoidBoolAndCards cardMatch;
	Queue<ScoreText> scoreTextQueue = new Queue<ScoreText>();
	Vector2[] pos;
	Vector2 shiftAmount = new Vector2(-100, 50);
	float appearDuration = 0.3f;
	float dealTime = 0.5f;
	int cardCountOnTable;
	Card[] cards;
	Queue<Card> waitForCompare = new Queue<Card>();

	public override IEnumerator Init()
	{
		ToggleMask(true);

		CardArraySetting setting = CardArrayManager.GetCurrentLevelSetting();
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
			cards[i].Init(CanFlipCardNow, CheckCardMatch);
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
	}

	public IEnumerator DealCard()
	{
		Sprite[] thisTimeCardImage = GetCardImage(cards.Length);
		float delayDuration = dealTime / cards.Length;
		for(int i = 0 ; i < cards.Length ; ++i)
		{
			cards[i].SetCard(cardBack[0], cardFace[0], thisTimeCardImage[i], Card.CardState.Back, thisTimeCardImage[i].name);
			cards[i].Appear(pos[i], shiftAmount, delayDuration * i, appearDuration);
		}
		cardCountOnTable = cards.Length;
		yield return new WaitForSeconds(dealTime);
	}

	public void FlipAllCard()
	{
		foreach(Card card in cards)
			card.Flip();
	}

	public void ToggleCardGlow(bool turnOn)
	{
		foreach(Card card in cards)
			card.ToggleCardGlow(turnOn);
	}

	bool CanFlipCardNow()
	{
		return true;
	}

	void CheckCardMatch(Card card)
	{
		waitForCompare.Enqueue(card);
		if(waitForCompare.Count > 1)
		{
			Card cardA = waitForCompare.Dequeue();
			Card cardB = waitForCompare.Dequeue();
			bool isMatch = false;
			if(cardA.GetCardId() == cardB.GetCardId())
			{
				AudioManager.Instance.PlayOneShot("GamePlayGetPair");
				isMatch = true;
				cardA.Match();
				cardB.Match();
				cardCountOnTable -= 2;
			}
			else
			{
				cardA.MisMatch();
				cardB.MisMatch();
			}
			if(cardMatch != null)
				cardMatch(isMatch, cardA, cardB);
		}
		if(cardCountOnTable == 0 && completeOneRound != null)
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
