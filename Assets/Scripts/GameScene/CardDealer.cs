using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardDealer : MonoBehaviour {
	public GameObject cardPrefab;
    public Transform cardParent;
    public Sprite[] cardBack;
	public Sprite[] cardFace;
	public Sprite[] cardImage;
	Card[] cards;
	Vector2[] pos;
	Vector2 shiftAmount = new Vector2(-100, 50);
	float appearDuration = 0.3f;
	float dealTime = 0.5f;
	Queue<Card> waitForCompare = new Queue<Card>();
	List<Card> cardsOnTheTable = new List<Card>();
	VoidNoneParameter completeOneRound;
    VoidBoolAndCards cardMatch;

    public void Init(CardArraySetting setting, VoidNoneParameter completeOneRound, VoidBoolAndCards cardMatch)
    {
        this.cardMatch = cardMatch;
        this.completeOneRound = completeOneRound;
        cards = new Card[setting.column*setting.row];
		pos = new Vector2[cards.Length];

        for (int i = 0 ; i < cards.Length ; ++i)
		{
			GameObject tmp = Instantiate(cardPrefab) as GameObject;
			tmp.name = cardPrefab.name + i.ToString();
			tmp.transform.SetParent(cardParent);
			tmp.transform.localScale = Vector3.one;
			tmp.SetActive(false);
			cards[i] = tmp.GetComponent<Card>();
			cards[i].SetSize(setting.edgeLength);
			cards[i].Init(UserFlipCard);
			float x = setting.realFirstPosition.x + (i%setting.column)*(setting.edgeLength + setting.cardGap);
			float y = setting.realFirstPosition.y - (i/setting.column)*(setting.edgeLength + setting.cardGap);
			pos[i] = new Vector2(x, y);
		}
        cardPrefab = null;
	}

	public IEnumerator DealCard()
	{		
		Sprite[] thisTimeCardImage = GetCardImage(cards.Length);
		float delayDuration = dealTime/cards.Length;
		for(int i = 0 ; i < cards.Length ; ++i)
		{
			cards[i].SetCard(cardBack[0], cardFace[0], thisTimeCardImage[i], Card.CardState.Back, thisTimeCardImage[i].name);
			cards[i].Appear(pos[i], shiftAmount, delayDuration*i, appearDuration);
            cardsOnTheTable.Add(cards[i]);
		}
		yield return new WaitForSeconds(dealTime);
	}

	public void FlipAllCard()
	{
		for(int i = 0 ; i < cards.Length ; ++i)
		{
			cards[i].Flip();
		}
	}
    
    public void ToggleCardGlow(bool turnOn)
    {
        foreach (Card usingCard in cards)
            usingCard.ToggleCardGlow(turnOn);
    }

	void UserFlipCard(Card card)
	{
		if(GameSceneController.currentState == GameSceneController.GameState.Playing)
		{
			waitForCompare.Enqueue(card);
			if(waitForCompare.Count > 1)
			{
				Card cardA = waitForCompare.Dequeue();
				Card cardB = waitForCompare.Dequeue();
                bool isMatch = false;
				if(cardA.GetCardId() == cardB.GetCardId())
				{
                    isMatch = true;
                    cardA.Match();
					cardB.Match();
					cardsOnTheTable.Remove(cardA);
					cardsOnTheTable.Remove(cardB);
				}else
				{
                    cardA.MisMatch();
					cardB.MisMatch();
                }
                if (cardMatch != null)
                    cardMatch(isMatch, cardA, cardB);
            }
			if(cardsOnTheTable.Count == 0 && completeOneRound != null)
				completeOneRound();
		}
	}
    
	Sprite[] GetCardImage(int count)
	{
		Sprite[] choosenCardFace = new Sprite[count];
		if(cardImage.Length > count/2)
		{
			int[] chooseCardIndex = new int[count/2];
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
				choosenCardFace[i] = cardImage[chooseCardIndex[i/2]];
			}
		}else if(cardImage.Length == count/2)
		{
			for(int i = 0 ; i < count ; ++i)
			{
				choosenCardFace[i] = cardImage[i/2];
			}
		}else
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
}
