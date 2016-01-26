using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public enum CardState { Face, Back, None }
public class CardBase : MonoBehaviour {
	Button thisButton;
	CanvasGroup thisCanvasGroup;
	RectTransform thisRectTransform;
	Image image_Glow;
	Image image_CardBody;
	Image image_CardImage;
	Image image_GoldCardFrame;
	Sprite cardFaceSprite;
	Sprite cardBackSprite;
	Sprite cardFaceImageSprite;
	CardState currentState = CardState.None;
	string cardId;
	bool isGoldCard;
	Coroutine flipAnimation;
	Coroutine matchEffect;
	Coroutine mismatchEffect;
	BoolCardBase checkCanFlipCard;
	VoidCardBase flipFinish;
	Color glowColor = new Color(1f, 0.85f, 0f);
	Vector3 flipDown = new Vector3(0f, 0.9f, 1f);

	public void Init(BoolCardBase checkCanFlipCard, VoidCardBase flipFinish, float cardSize)
	{
		this.checkCanFlipCard = checkCanFlipCard;
		this.flipFinish = flipFinish;

		thisButton = this.GetComponent<Button>();
		thisButton.onClick.AddListener(FlipByUser);
		thisRectTransform = this.GetComponent<RectTransform>();
		thisCanvasGroup = this.GetComponent<CanvasGroup>();
		isGoldCard = false;

		image_Glow = this.transform.Find("Image_Glow").GetComponent<Image>();
		image_CardBody = this.GetComponent<Image>();
		image_CardImage = this.transform.Find("Image_CardImage").GetComponent<Image>();
		image_GoldCardFrame = this.transform.Find("Image_GoldCardFrame").GetComponent<Image>();

		image_GoldCardFrame.gameObject.SetActive(false);
		image_Glow.gameObject.SetActive(false);

		cardBackSprite = InventoryManager.Instance.GetCurrentCardBack();
		cardFaceSprite = InventoryManager.Instance.GetCurrentCardFace();

		image_CardBody.rectTransform.sizeDelta = Vector2.one * cardSize;
		image_GoldCardFrame.rectTransform.sizeDelta = (new Vector2(image_GoldCardFrame.sprite.rect.width, image_GoldCardFrame.sprite.rect.height)) * cardSize / 192f;
		image_Glow.rectTransform.sizeDelta = Vector2.one * (cardSize + 48f);
	}

	public void SetSprite(Sprite cardFaceImage, CardState defaultState)
	{
		cardFaceImageSprite = cardFaceImage;
		SetCardState(defaultState);
	}

	public void SetCardId(string cardId)
	{
		this.cardId = cardId;
	}

	public void FlipBySystem()
	{
		if(flipAnimation != null)
			StopCoroutine(flipAnimation);
		flipAnimation = StartCoroutine(FlipAniamtion(false));
	}

	public void DealCard(Vector2 pos, Vector2 shiftAmount, float delayTime, float duration)
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
		} else
			image_GoldCardFrame.gameObject.SetActive(false);
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
		} else
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

	public string GetCardId()
	{
		return cardId;
	}

	public bool IsGoldCard()
	{
		return isGoldCard;
	}

	void FlipByUser()
	{
		if(checkCanFlipCard(this))
		{
			AudioManager.Instance.PlayOneShot("GamePlayChooseCard");
			if(flipAnimation != null)
				StopCoroutine(flipAnimation);
			flipAnimation = StartCoroutine(FlipAniamtion(true));
		}
	}

	void SetCardState(CardState state)
	{
		if(currentState != state)
		{
			currentState = state;
			switch(state)
			{
				case CardState.Back:
					image_CardImage.gameObject.SetActive(false);
					if(isGoldCard)
						image_GoldCardFrame.gameObject.SetActive(true);
					else
						image_GoldCardFrame.gameObject.SetActive(false);
					thisButton.interactable = true;
					image_CardBody.sprite = cardBackSprite;
					break;
				case CardState.Face:
					image_GoldCardFrame.gameObject.SetActive(false);
					image_CardImage.sprite = cardFaceImageSprite;
					image_CardImage.gameObject.SetActive(true);
					image_CardImage.rectTransform.sizeDelta = (new Vector2(cardFaceImageSprite.rect.width, cardFaceImageSprite.rect.height)) * image_CardBody.rectTransform.sizeDelta.x / 192f;
					thisButton.interactable = false;
					image_CardBody.sprite = cardFaceSprite;
					break;
			}
		}
	}

	IEnumerator FlipAniamtion(bool flipByUser)
	{
		image_CardBody.DOColor(Color.gray, 0.2f).SetEase(Ease.OutQuad);
		image_CardImage.DOColor(Color.gray, 0.2f).SetEase(Ease.OutQuad);
		yield return thisRectTransform.DOScale(flipDown, 0.2f).SetEase(Ease.OutQuad).WaitForCompletion();

		switch(currentState)
		{
			case CardState.Back:
				SetCardState(CardState.Face);
				break;
			case CardState.Face:
				SetCardState(CardState.Back);
				break;
		}

		image_CardBody.color = Color.white;
		image_CardImage.color = Color.white;
		yield return thisRectTransform.DOScale(Vector3.one, 0.15f).WaitForCompletion();

		if(currentState == CardState.Face && flipByUser && flipFinish != null)
			flipFinish(this);

		flipAnimation = null;
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
		FlipBySystem();

		mismatchEffect = null;
	}
}
