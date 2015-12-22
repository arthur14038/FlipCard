using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PressButtonTool : ButtonOffset
{
	public Image image_Back;
	public Image image_Icon;
	public CanvasGroup thisGroup;
	public VoidPressButtonTool onLightUp;
	public enum PressButtonState {Clickable, Disable, LightUp}
	public PressButtonState ButtonState
	{
		get
		{
			return currentState;
		}
	}
	PressButtonState currentState;
	Tweener thisTweener;

	public override void OnPointerDown(PointerEventData eventData)
	{
		if(currentState == PressButtonState.Clickable)
		{
			base.OnPointerDown(eventData);
			if(thisTweener != null)
				thisTweener.Kill();
			thisTweener = image_Back.DOFillAmount(1f, 1f).OnComplete(
				delegate () {
					UpdateButton(PressButtonState.LightUp);
					if(onLightUp != null)
						onLightUp(this);
				}
			);
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		if(currentState == PressButtonState.Clickable)
		{
			if(thisTweener != null)
				thisTweener.Kill();
			thisTweener = image_Back.DOFillAmount(0f, 0.5f);
		}
	}

	public void UpdateButton(PressButtonState state)
	{
		currentState = state;
		thisGroup.alpha = 1f;
		image_Icon.color = Color.grey;
		switch(currentState)
		{
			case PressButtonState.Disable:
				image_Back.fillAmount = 0f;
				thisGroup.alpha = 0.5f;
				break;
			case PressButtonState.LightUp:
				image_Icon.color = Color.white;
				break;
		}
	}
}
