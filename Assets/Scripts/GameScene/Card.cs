using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class Card : MonoBehaviour {
	public Image card;
	public Text text;
	public enum CardState{Face, Back, None}
	CardState currentState = CardState.None;
	Sprite cardBack;
	Sprite cardFace;
	Coroutine flipCard;
	Coroutine matchEffect;
	Coroutine mismatchEffect;
	Color transparentColor = new Color(1, 1, 1, 0);
	string cardId;
	Button thisButton;
	VoidCard flipToFace;

	public void Init(VoidCard flipToFace)
	{
		this.flipToFace = flipToFace;
		thisButton = this.GetComponent<Button>();
	}

	public void SetSize(float edgeLength)
	{
		card.rectTransform.sizeDelta = new Vector2(edgeLength, edgeLength);
	}

	public void SetCard(Sprite cardBack, Sprite cardFace, CardState defaultState, string cardId)
	{
		text.text = cardId;
		this.cardId = cardId;
		this.cardBack = cardBack;
		this.cardFace = cardFace;
		SetImageAndState(defaultState);
	}

	public void Appear(Vector2 pos, Vector2 shiftAmount, float delayTime, float duration)
	{
		if(!this.gameObject.activeSelf)
			this.gameObject.SetActive(true);

		card.color = transparentColor;
		card.rectTransform.anchoredPosition = pos + shiftAmount;
		card.DOFade(1f, duration).SetDelay(delayTime);
		card.rectTransform.DOAnchorPos(pos, duration).SetDelay(delayTime);
	}

	public void Flip()
	{
		if(flipCard != null)
			StopCoroutine(flipCard);
		flipCard = StartCoroutine(FlipCard());
	}

	public void Match()
	{
		if(matchEffect != null)
			StopCoroutine(matchEffect);
		matchEffect = StartCoroutine(MatchEffect());
	}

	public void MisMatch()
	{
		if(mismatchEffect != null)
			StopCoroutine(mismatchEffect);
		mismatchEffect = StartCoroutine(MismatchEffect());
	}

	public string GetCardId()
	{
		return cardId;
	}

	IEnumerator FlipCard()
	{
		card.DOColor(Color.gray, 0.2f).SetEase(Ease.OutQuad);
		yield return card.rectTransform.DOScale(new Vector3(0f, 0.9f, 1f), 0.2f).SetEase(Ease.OutQuad).WaitForCompletion();

		switch(currentState)
		{
		case CardState.Back:
			SetImageAndState(CardState.Face);
			break;
		case CardState.Face:
			SetImageAndState(CardState.Back);
			break;
		}

		card.color = Color.white;
		yield return card.rectTransform.DOScale(Vector3.one, 0.15f).WaitForCompletion();

		if(currentState == CardState.Face)			
			if(flipToFace != null)
				flipToFace(this);

		flipCard = null;
	}

	IEnumerator MatchEffect()
	{
		yield return card.rectTransform.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
				
		this.gameObject.SetActive(false);
		card.rectTransform.localScale = Vector3.one;

		matchEffect = null;
	}

	IEnumerator MismatchEffect()
	{
		yield return new WaitForSeconds(0.2f);
		Flip();
		
		mismatchEffect = null;
	}

	void SetImageAndState(CardState state)
	{
		currentState = state;
		switch(currentState)
		{
		case CardState.Back:
			thisButton.interactable = true;
			card.sprite = cardBack;
			break;
		case CardState.Face:
			thisButton.interactable = false;
			card.sprite = cardFace;
			break;
		}
	}
}
