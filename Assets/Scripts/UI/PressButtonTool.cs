using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PressButtonTool : ButtonOffset
{
	public Image image_Back;
	public Image image_Icon;
	public CanvasGroup thisGroup;
	public GameObject lightUpParticle;
	public GameObject clickableParticle;
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
			thisTweener = image_Back.DOFillAmount(1f, 0.8f).OnComplete(
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
		clickableParticle.SetActive(true);
		lightUpParticle.SetActive(false);
		currentState = state;
		thisGroup.alpha = 1f;
		image_Icon.color = Color.grey;
		switch(currentState)
		{
			case PressButtonState.Disable:
				clickableParticle.SetActive(false);
				image_Back.fillAmount = 0f;
				thisGroup.alpha = 0.5f;
				break;
			case PressButtonState.LightUp:
				lightUpParticle.SetActive(true);
				image_Icon.color = Color.white;
				break;
		}
	}
}
