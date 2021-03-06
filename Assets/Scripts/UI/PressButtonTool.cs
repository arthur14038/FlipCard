﻿using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PressButtonTool : ButtonOffset
{
	public Image image_Back;
	public Image image_Icon;
	public Image image_AdditionCircle;
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
	Tweener additionTweener;
	float chargeTime = 0f;

	public void SetChargeTime(float value)
	{
		chargeTime = value;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if(currentState == PressButtonState.Clickable)
		{
			AudioManager.Instance.PlayOneShot("Button_Click");
			base.OnPointerDown(eventData);
			image_AdditionCircle.gameObject.SetActive(true);
			image_AdditionCircle.fillAmount = 0f;
			if(chargeTime > 0f)
			{
				if(thisTweener != null)
					thisTweener.Kill();
				thisTweener = image_Back.DOFillAmount(1f, chargeTime).OnComplete(
					delegate () {
						UpdateButton(PressButtonState.LightUp);
						if(onLightUp != null)
							onLightUp(this);
					}
				);
				additionTweener = image_AdditionCircle.DOFillAmount(1f, chargeTime);
			}
			else
			{
				image_AdditionCircle.fillAmount = 1f;
                image_Back.fillAmount = 1f;
				UpdateButton(PressButtonState.LightUp);
				if(onLightUp != null)
					onLightUp(this);
			}
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		if(currentState == PressButtonState.Clickable)
		{
			if(additionTweener != null)
				additionTweener.Kill();
			if(thisTweener != null)
				thisTweener.Kill();
			image_AdditionCircle.gameObject.SetActive(false);
			thisTweener = image_Back.DOFillAmount(0f, 0.5f);
			additionTweener = image_AdditionCircle.DOFillAmount(0f, 0.5f);
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
				image_AdditionCircle.gameObject.SetActive(false);
				image_Back.fillAmount = 0f;
				thisGroup.alpha = 0.5f;
				break;
			case PressButtonState.LightUp:
				clickableParticle.SetActive(false);
				lightUpParticle.SetActive(true);
				image_Icon.color = Color.white;
				break;
		}
	}
}
