﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class Card : MonoBehaviour {
	public enum CardType {Normal, Lucky}
	public Image cardBody;
	public Image cardImage;
    public Image image_Glow;
    public Text text_CardId;
	public Color glowColor = new Color(1f, 0.85f, 0f);
	public GameObject luckyEffect;
	public enum CardState{Face, Back, None}
	CardState currentState = CardState.None;
	CardType thisCardType;
	Sprite cardBack;
	Sprite cardFace;
	Coroutine flipCard;
	Coroutine matchEffect;
	Coroutine mismatchEffect;
	Button thisButton;
	BoolCard checkCanFlipCard;
	VoidCard flipFinish;
	Color transparentColor = new Color(1, 1, 1, 0);
	Vector3 flipDown = new Vector3(0f, 0.9f, 1f);
	string cardId;
	bool isLuckyCard;

	public void Init(BoolCard checkCanFlipCard, VoidCard flipFinish)
	{
		this.checkCanFlipCard = checkCanFlipCard;
		this.flipFinish = flipFinish;
		luckyEffect.SetActive(false);
		image_Glow.gameObject.SetActive(false);
		thisButton = this.GetComponent<Button>();
    }
	
	public void SetSize(float edgeLength)
	{
		cardBody.rectTransform.sizeDelta = Vector2.one*edgeLength;
        image_Glow.rectTransform.sizeDelta = Vector2.one * (edgeLength + 48f);
    }

	public void SetCard(Sprite cardBack, Sprite cardFace, Sprite cardImageSprite, CardState defaultState, CardType type)
	{
        text_CardId.text = cardImageSprite.name.Replace("CardImage_", "");
		thisCardType = type;
		if(thisCardType == CardType.Lucky)
			luckyEffect.SetActive(true);
        this.cardId = cardImageSprite.name;
		this.cardBack = cardBack;
		this.cardFace = cardFace;
		cardImage.sprite = cardImageSprite;
		cardImage.rectTransform.sizeDelta = (new Vector2(cardImageSprite.rect.width, cardImageSprite.rect.height))*cardBody.rectTransform.sizeDelta.x/192f;
		SetImageAndState(defaultState);
	}

	public void Appear(Vector2 pos, Vector2 shiftAmount, float delayTime, float duration)
	{
		if(!this.gameObject.activeSelf)
			this.gameObject.SetActive(true);
        
        cardBody.color = transparentColor;
		cardBody.rectTransform.anchoredPosition = pos + shiftAmount;
		cardBody.DOFade(1f, duration).SetDelay(delayTime);
		cardBody.rectTransform.DOAnchorPos(pos, duration).SetDelay(delayTime);

        if(image_Glow.gameObject.activeSelf)
        {
            image_Glow.color = glowColor - Color.black;
            image_Glow.DOFade(1f, duration).SetDelay(delayTime);
        }
    }

	public void Flip(bool flipByUser = false)
	{
		if(flipByUser)
		{
			if(checkCanFlipCard(this))
			{
				AudioManager.Instance.PlayOneShot("GamePlayChooseCard");
				if(flipCard != null)
					StopCoroutine(flipCard);
				flipCard = StartCoroutine(FlipCard(flipByUser));
			}
		}
		else
		{
			if(flipCard != null)
				StopCoroutine(flipCard);
			flipCard = StartCoroutine(FlipCard(flipByUser));
		}
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

    public void ToggleCardGlow(bool turnOn)
    {
        if(turnOn)
        {
            image_Glow.gameObject.SetActive(true);
            if(this.gameObject.activeInHierarchy)
            {
                image_Glow.color = glowColor - Color.black;
                image_Glow.DOFade(1f, 0.3f);
            }
            else
            {
                image_Glow.color = glowColor;
            }
        }
        else
		{
			if(this.gameObject.activeInHierarchy)
			{
				image_Glow.DOFade(0f, 0.3f).OnComplete(
					delegate () {
						image_Glow.gameObject.SetActive(false);
					}
					);
			}
			else
			{
				image_Glow.gameObject.SetActive(false);
			}
		}
    }

    public Vector2 GetAnchorPosition()
    {
        return cardBody.rectTransform.anchoredPosition;
    }

	public CardType GetCardType()
	{
		return thisCardType;
	}

	IEnumerator FlipCard(bool flipByUser)
	{
		cardBody.DOColor(Color.gray, 0.2f).SetEase(Ease.OutQuad);
		yield return cardBody.rectTransform.DOScale(flipDown, 0.2f).SetEase(Ease.OutQuad).WaitForCompletion();

		switch(currentState)
		{
		case CardState.Back:
			SetImageAndState(CardState.Face);
			break;
		case CardState.Face:
			SetImageAndState(CardState.Back);
			break;
		}

		cardBody.color = Color.white;
		yield return cardBody.rectTransform.DOScale(Vector3.one, 0.15f).WaitForCompletion();

		if(currentState == CardState.Face && flipByUser && flipFinish != null)
			flipFinish(this);

		flipCard = null;
	}

	IEnumerator MatchEffect()
	{
		yield return cardBody.rectTransform.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();
        
        this.gameObject.SetActive(false);
		cardBody.rectTransform.localScale = Vector3.one;

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
			cardImage.gameObject.SetActive(false);
			thisButton.interactable = true;
			cardBody.sprite = cardBack;
			break;
		case CardState.Face:
			cardImage.gameObject.SetActive(true);
			thisButton.interactable = false;
			cardBody.sprite = cardFace;
			break;
		}
	}
}
