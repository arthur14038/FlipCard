using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public enum CardState { Face, Back, None }
public class CardBase : MonoBehaviour {
	protected Button thisButton;
	protected CanvasGroup thisCanvasGroup;
	protected RectTransform thisRectTransform;
	protected Image image_Glow;
	protected Image image_CardBody;
	protected Image image_CardImage;
	protected Image image_GoldCardFrame;
	protected Sprite cardFaceSprite;
	protected Sprite cardBackSprite;
	protected Sprite cardFaceImageSprite;
	protected CardState currentState = CardState.None;
	protected string cardId;
	protected bool isGoldCard;
	protected Coroutine flipAnimation;
	protected Coroutine matchEffect;
	protected Coroutine mismatchEffect;
	protected BoolCardBase checkCanFlipCard;
	protected VoidCardBase flipFinish;
	protected Color glowColor = new Color(1f, 0.85f, 0f);
	protected Vector3 flipDown = new Vector3(0f, 0.9f, 1f);
	protected float standardCardSize = 192f;
	protected float currentCardSize;
	protected Tweener flareEffectTweener;

	public bool IsBombCard
	{
		get
		{
			return isBombCard;
        }
	}
	protected bool isBombCard = false;
	
	public bool IsFlareCard
	{
		get
		{
			return isFlareCard;
		}
	}
	protected bool isFlareCard = false;

	public virtual void Init(BoolCardBase checkCanFlipCard, VoidCardBase flipFinish)
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
	}

	public virtual void SetSprite(Sprite cardFaceImage, CardState defaultState)
	{
		cardFaceImageSprite = cardFaceImage;
		SetCardState(defaultState);
	}

	public void SetCardId(string cardId)
	{
		this.cardId = cardId;
		if(cardId.EndsWith("Bomb"))
			isBombCard = true;
		else
			isBombCard = false;
		
		if(cardId.EndsWith("Flare"))
			isFlareCard = true;
		else
			isFlareCard = false;
	}

	public virtual void FlipBySystem()
	{
		if(flipAnimation != null)
			StopCoroutine(flipAnimation);
		flipAnimation = StartCoroutine(FlipAniamtion(false));
	}

	public void DealCard(Vector2 pos, Vector2 shiftAmount, float delayTime, float duration, float cardSize)
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
		
		if(currentCardSize != cardSize)
		{
			currentCardSize = cardSize;
			image_CardBody.rectTransform.sizeDelta = Vector2.one * currentCardSize;
			image_Glow.rectTransform.sizeDelta = Vector2.one * (currentCardSize + 48f);
			image_GoldCardFrame.rectTransform.sizeDelta = (new Vector2(image_GoldCardFrame.sprite.rect.width, image_GoldCardFrame.sprite.rect.height)) * currentCardSize / standardCardSize;
			image_CardImage.rectTransform.sizeDelta = (new Vector2(image_CardImage.sprite.rect.width, image_CardImage.sprite.rect.height)) * currentCardSize / standardCardSize;
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
		if(image_Glow.gameObject.activeSelf != turnOn)
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
	}

	public void ToggleGoldCard(bool value)
	{
		if(isGoldCard != value)
			isGoldCard = value;
		else
			return;

		switch(currentState)
		{
			case CardState.Back:
				if(isGoldCard)
				{
					image_GoldCardFrame.gameObject.SetActive(true);
					if(image_GoldCardFrame.gameObject.activeInHierarchy)
					{
						image_GoldCardFrame.color = Color.white - Color.black;
						image_GoldCardFrame.DOFade(1f, 0.3f);
					}else
					{
						image_GoldCardFrame.color = Color.white;
					}
				} else
				{
					if(image_GoldCardFrame.gameObject.activeInHierarchy)
					{
						image_GoldCardFrame.DOFade(0f, 0.3f).OnComplete(
					delegate () {
						image_GoldCardFrame.gameObject.SetActive(false);
					}
					);
					}else
					{
						image_GoldCardFrame.gameObject.SetActive(false);
					}
				}
				break;
			case CardState.Face:
				image_GoldCardFrame.color = Color.white;
				image_GoldCardFrame.gameObject.SetActive(false);
				break;
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

	public virtual IEnumerator FlareEffect()
	{
		yield return new WaitForSeconds(0.15f);
		SetCardImage(cardFaceImageSprite);
		flareEffectTweener = image_CardImage.DOFade(0f, 3.5f).SetEase(Ease.InQuad);
	}
	
	void FlipByUser()
	{
		if(checkCanFlipCard != null)
		{
			if(checkCanFlipCard(this))
			{
				AudioManager.Instance.PlayOneShot("GamePlayChooseCard");
				if(flipAnimation != null)
					StopCoroutine(flipAnimation);
				flipAnimation = StartCoroutine(FlipAniamtion(true));
			}
		}
	}

	protected virtual void SetCardState(CardState state)
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
					SetCardImage(cardFaceImageSprite);
					thisButton.interactable = false;
					image_CardBody.sprite = cardFaceSprite;
					break;
			}
		}
	}

	protected void SetCardImage(Sprite imageSprite)
	{
		if(flareEffectTweener != null)
		{
			flareEffectTweener.Kill();
			flareEffectTweener = null;
		}
		image_CardImage.color = Color.white;
		image_CardImage.sprite = imageSprite;
		image_CardImage.rectTransform.sizeDelta = (new Vector2(imageSprite.rect.width, imageSprite.rect.height)) * currentCardSize / standardCardSize;
		if(!image_CardImage.gameObject.activeSelf)
			image_CardImage.gameObject.SetActive(true);
	}

	protected IEnumerator FlipAniamtion(bool flipByUser)
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
		if(flipAnimation != null)
			StopCoroutine(flipAnimation);
		flipAnimation = StartCoroutine(FlipAniamtion(false));

		mismatchEffect = null;
	}
}
