using UnityEngine;
using System.Collections;
using DG.Tweening;

public class UnknownCard : CardBase {
	Sprite unknownSprite;

	public override void Init(BoolCardBase checkCanFlipCard, VoidCardBase flipFinish)
	{
		base.Init(checkCanFlipCard, flipFinish);
		unknownSprite = Resources.Load<Sprite>("CardImage/CardImage_Unknown");
	}

	public override void FlipBySystem()
	{
	}

	protected override void SetCardState(CardState state)
	{
		if(currentState != state)
		{
			currentState = state;
			switch(state)
			{
				case CardState.Back:
					if(isGoldCard)
						image_GoldCardFrame.gameObject.SetActive(true);
					else
						image_GoldCardFrame.gameObject.SetActive(false);
					SetCardImage(unknownSprite);
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

}
