using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public enum CardType { Normal, UnknownBack, UnknownFace }
public enum CardState { Face, Back, None }
public class Card_Normal : MonoBehaviour
{
	public Image image_Glow;
	public Image image_CardBody;
	public Image image_CardImage;
	public Image image_GoldCardFrame;
	public Color glowColor = new Color(1f, 0.85f, 0f);
	public Sprite unknownSprite;
	CardType currentType;
	CardState currentState;
	Sprite cardBackSprite;
	Sprite cardFaceSprite;
	Sprite cardBackImageSprite;
	Sprite cardFaceImageSprite;
	Coroutine flipCard;
	Coroutine matchEffect;
	Coroutine mismatchEffect;
	Button thisButton;
	RectTransform thisRectTransform;
	CanvasGroup thisCanvasGroup;
    BoolCard_Normal checkCanFlipCard;
	VoidCard_Normal flipFinish;
	Color transparentColor = new Color(1, 1, 1, 0);
	Vector3 flipDown = new Vector3(0f, 0.9f, 1f);
	string cardId;
	bool isGoldCard;

	public void Init(BoolCard_Normal checkCanFlipCard, VoidCard_Normal flipFinish, float edgeLength)
	{
		this.checkCanFlipCard = checkCanFlipCard;
		this.flipFinish = flipFinish;
		image_GoldCardFrame.gameObject.SetActive(false);
		image_Glow.gameObject.SetActive(false);
		thisButton = this.GetComponent<Button>();
		thisRectTransform = this.GetComponent<RectTransform>();
		thisCanvasGroup = this.GetComponent<CanvasGroup>();
		isGoldCard = false;

		cardBackSprite = InventoryManager.Instance.GetCurrentCardBack();
		cardFaceSprite = InventoryManager.Instance.GetCurrentCardFace();

		image_CardBody.rectTransform.sizeDelta = Vector2.one * edgeLength;
		image_GoldCardFrame.rectTransform.sizeDelta = (new Vector2(image_GoldCardFrame.sprite.rect.width, image_GoldCardFrame.sprite.rect.height)) * edgeLength / 192f;
		image_Glow.rectTransform.sizeDelta = Vector2.one * (edgeLength + 48f);
	}

	public void SetCardId(string cardId)
	{
		this.cardId = cardId;
	}

	public void SetCardType(CardType type)
	{
		currentType = type;
		switch(currentType)
		{
			case CardType.UnknownBack:
				if(currentState == CardState.Back)
					SetCardImage(unknownSprite);
				break;
			case CardType.UnknownFace:
				if(currentState == CardState.Face)
					SetCardImage(unknownSprite);
				break;
			case CardType.Normal:
				if(currentState == CardState.Back)
					SetCardImage(cardBackImageSprite);
				if(currentState == CardState.Face)
					SetCardImage(cardFaceImageSprite);
				break;
		}
	}

	public void SetCardSprite(Sprite cardFaceImageSprite, Sprite cardBackImageSprite, CardState defaultState)
	{
		this.cardBackImageSprite = cardBackImageSprite;
		this.cardFaceImageSprite = cardFaceImageSprite;
		SetImageAndState(defaultState);
	}

	public void Appear(Vector2 pos, Vector2 shiftAmount, float delayTime, float duration)
	{
		if(!this.gameObject.activeSelf)
			this.gameObject.SetActive(true);

		thisCanvasGroup.alpha = 0;
		thisCanvasGroup.DOFade(1f, duration).SetDelay(delayTime);
		thisRectTransform.anchoredPosition = pos + shiftAmount;
		thisRectTransform.DOAnchorPos(pos, duration).SetDelay(delayTime);

		if(isGoldCard)
		{
			image_GoldCardFrame.gameObject.SetActive(true);
			image_GoldCardFrame.color = Color.white;
        }
		else
			image_GoldCardFrame.gameObject.SetActive(false);
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
		} else
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

	public void ToggleCardGlow(bool turnOn)
	{
		if(turnOn)
		{
			image_Glow.gameObject.SetActive(true);
			if(this.gameObject.activeInHierarchy)
			{
				image_Glow.color = glowColor - Color.black;
				image_Glow.DOFade(1f, 0.3f);
			} else
			{
				image_Glow.color = glowColor;
			}
		} else
		{
			if(this.gameObject.activeInHierarchy)
			{
				image_Glow.DOFade(0f, 0.3f).OnComplete(
					delegate () {
						image_Glow.gameObject.SetActive(false);
					}
					);
			} else
			{
				image_Glow.gameObject.SetActive(false);
			}
		}
	}

	public void ToggleGoldCard(bool value)
	{
		isGoldCard = value;
        if(isGoldCard)
		{
			switch(currentState)
			{
				case CardState.Back:
					image_GoldCardFrame.gameObject.SetActive(true);
					image_GoldCardFrame.color = Color.white - Color.black;
					image_GoldCardFrame.DOFade(1f, 0.3f);
					break;
				case CardState.Face:
					image_GoldCardFrame.color = Color.white;
					image_GoldCardFrame.gameObject.SetActive(false);
					break;
			}
		}else
		{
			switch(currentState)
			{
				case CardState.Back:
					image_GoldCardFrame.DOFade(0f, 0.3f).OnComplete(
					delegate () {
						image_GoldCardFrame.gameObject.SetActive(false);
					}
					);
					break;
				case CardState.Face:
					image_GoldCardFrame.gameObject.SetActive(false);
					break;
			}
		}
	}

	public Vector2 GetAnchorPosition()
	{
		return thisRectTransform.anchoredPosition;
	}

	public CardType GetCardType()
	{
		return currentType;
	}

	public string GetCardId()
	{
		return cardId;
	}

	public bool IsGoldCard()
	{
		return isGoldCard;
	}

	IEnumerator FlipCard(bool flipByUser)
	{
		image_CardBody.DOColor(Color.gray, 0.2f).SetEase(Ease.OutQuad);
		image_CardImage.DOColor(Color.gray, 0.2f).SetEase(Ease.OutQuad);
		yield return thisRectTransform.DOScale(flipDown, 0.2f).SetEase(Ease.OutQuad).WaitForCompletion();

		switch(currentState)
		{
			case CardState.Back:
				SetImageAndState(CardState.Face);
				break;
			case CardState.Face:
				SetImageAndState(CardState.Back);
				break;
		}

		image_CardBody.color = Color.white;
		image_CardImage.color = Color.white;
		yield return thisRectTransform.DOScale(Vector3.one, 0.15f).WaitForCompletion();

		if(currentState == CardState.Face && flipByUser && flipFinish != null)
			flipFinish(this);

		flipCard = null;
	}

	IEnumerator MatchEffect()
	{
		yield return thisRectTransform.DOScale(0f, 0.3f).SetEase(Ease.InBack).WaitForCompletion();

		this.gameObject.SetActive(false);
		thisRectTransform.localScale = Vector3.one;

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
				switch(currentType)
				{
					case CardType.UnknownBack:
						SetCardImage(unknownSprite);
						break;
					default:
						SetCardImage(cardBackImageSprite);
						break;
				}
				if(isGoldCard)
					image_GoldCardFrame.gameObject.SetActive(true);
				else
					image_GoldCardFrame.gameObject.SetActive(false);
                thisButton.interactable = true;
				image_CardBody.sprite = cardBackSprite;
				break;
			case CardState.Face:
				image_GoldCardFrame.gameObject.SetActive(false);
				switch(currentType)
				{
					case CardType.UnknownFace:
						SetCardImage(unknownSprite);
						break;
					default:
						SetCardImage(cardFaceImageSprite);
						break;
				}
				thisButton.interactable = false;
				image_CardBody.sprite = cardFaceSprite;
				break;
		}
	}

	void SetCardImage(Sprite theImage)
	{
		if(theImage == null)
		{
			image_CardImage.gameObject.SetActive(false);
		} else
		{
			image_CardImage.gameObject.SetActive(true);
			image_CardImage.sprite = theImage;
			image_CardImage.rectTransform.sizeDelta = (new Vector2(theImage.rect.width, theImage.rect.height)) * image_CardBody.rectTransform.sizeDelta.x / 192f;
		}
	}
}
