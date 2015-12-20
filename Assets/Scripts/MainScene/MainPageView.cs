﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class MainPageView : AbstractView {
	enum ViewState{Main, SettingWindow, LeaveWindow, UnderConstructionWindow, Close}
	ViewState currentState;
	public RectTransform group_Main;
    public Image image_Mask;
    public RectTransform image_SettingWindow;
	public RectTransform image_LeaveWindow;
	public RectTransform image_UnderConstructionWindow;
	public Toggle toggle_Music;
	public Toggle toggle_Sound;
	public VoidNoneParameter onClick1P;
	public VoidNoneParameter onClick2P;
	public VoidNoneParameter onClickRate;
	public VoidNoneParameter onClickMail;
	public VoidNoneParameter onClickShop;
	public VoidNoneParameter onClickLeaveGame;
	public VoidNoneParameter onClickNotify;

	public override IEnumerator Init ()
	{
		currentState = ViewState.Main;
		image_Mask.gameObject.SetActive(false);
		image_LeaveWindow.gameObject.SetActive(false);
		image_SettingWindow.gameObject.SetActive(false);
		image_UnderConstructionWindow.gameObject.SetActive(false);
		group_Main.gameObject.SetActive(true);
		escapeEvent = OnClickEscape;
		
		AudioManager.Instance.SetListenToToggle(false);
		toggle_Music.isOn = !PlayerPrefsManager.MusicSetting;
		toggle_Sound.isOn = !PlayerPrefsManager.SoundSetting;
		AudioManager.Instance.SetListenToToggle(true);
		yield return 0;
	}

	public void OnClick1P()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClick1P != null)
			onClick1P();
	}
	
	public void OnClick2P()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClick2P != null)
			onClick2P();
	}

	public void OnClickShop()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickShop != null)
			onClickShop();
	}

	public void OnClickRate()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickRate != null)
			onClickRate();
	}

	public void OnClickMail()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickMail != null)
			onClickMail();
	}

	public void OnClickLeaveGame()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickLeaveGame != null)
			onClickLeaveGame();
	}

	public void OnClickExitLeaveWindow()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		currentState = ViewState.Main;
		image_LeaveWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack);
		image_Mask.DOColor(Color.clear, 0.3f).OnComplete(
			delegate () {
				image_Mask.gameObject.SetActive(false);
				image_LeaveWindow.gameObject.SetActive(false);
			}
		);
	}

	public void OnClickSetting()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		currentState = ViewState.SettingWindow;
		image_Mask.gameObject.SetActive(true);
        image_Mask.color = Color.clear;
        image_Mask.DOColor(Color.black * 0.7f, 0.3f);
		image_SettingWindow.gameObject.SetActive(true);
        image_SettingWindow.anchoredPosition = hideDown;
        image_SettingWindow.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
	}

	public void OnClickExitSetting()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		currentState = ViewState.Main;
        image_SettingWindow.DOAnchorPos(hideDown, 0.3f).SetEase(Ease.InQuad);
        image_Mask.DOColor(Color.clear, 0.3f).OnComplete(
            delegate () {
                image_Mask.gameObject.SetActive(false);
				image_SettingWindow.gameObject.SetActive(false);
			}
        );
	}
    
	public void OnClickNotify()
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickNotify != null)
			onClickNotify();
	}

	public void ShowUnderConstruction()
	{
		currentState = ViewState.UnderConstructionWindow;
		image_Mask.gameObject.SetActive(true);
		image_Mask.color = Color.clear;
		image_Mask.DOColor(Color.black * 0.7f, 0.3f);
		image_UnderConstructionWindow.gameObject.SetActive(true);
		image_UnderConstructionWindow.localScale = Vector3.zero;
		image_UnderConstructionWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
	}

	public void OnClickExitUnderConstruction()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		currentState = ViewState.Main;
		image_UnderConstructionWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack);
		image_Mask.DOColor(Color.clear, 0.3f).OnComplete(
			delegate () {
				image_Mask.gameObject.SetActive(false);
				image_UnderConstructionWindow.gameObject.SetActive(false);
			}
		);
	}

	public void OnMusicValueChange(bool value)
	{
		AudioManager.Instance.MusicChangeValue(!value);
	}

	public void OnSoundValueChange(bool value)
	{
		AudioManager.Instance.SoundChangeValue(!value);
	}

	void OnClickEscape()
	{
		switch(currentState)
		{
		case ViewState.Main:
			AudioManager.Instance.PlayOneShot("Button_Click");
			currentState = ViewState.LeaveWindow;
			image_Mask.gameObject.SetActive(true);
			image_Mask.color = Color.clear;
			image_Mask.DOColor(Color.black * 0.7f, 0.3f);
			image_LeaveWindow.gameObject.SetActive(true);
			image_LeaveWindow.localScale = Vector3.zero;
			image_LeaveWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
			break;
		case ViewState.SettingWindow:
			OnClickExitSetting();
			break;
		case ViewState.LeaveWindow:
			OnClickExitLeaveWindow();
			break;
		case ViewState.UnderConstructionWindow:
			OnClickExitUnderConstruction();
			break;
		}
	}

	protected override IEnumerator HideUIAnimation ()
	{
		currentState = ViewState.Close;
		group_Main.anchoredPosition = Vector2.zero;
		yield return group_Main.DOAnchorPos(hideLeft, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation ()
	{
		currentState = ViewState.Close;
		group_Main.anchoredPosition = hideLeft;
		group_Main.gameObject.SetActive(true);
		image_Mask.gameObject.SetActive(false);

		yield return group_Main.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();

		currentState = ViewState.Main;
		showCoroutine = null;
	}
}
