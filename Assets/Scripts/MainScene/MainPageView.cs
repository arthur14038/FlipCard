using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class MainPageView : AbstractView {
	enum ViewState{Main, SettingWindow, LeaveWindow, Close}
	ViewState currentState;
	public RectTransform group_Main;
	public CanvasGroup image_Mask;
	public RectTransform image_SettingWindow;
	public RectTransform image_LeaveWindow;
	public VoidNoneParameter onClick1P;
	public VoidNoneParameter onClick2P;
	public VoidNoneParameter onClickRate;
	public VoidNoneParameter onClickMail;
	public VoidNoneParameter onClickShop;
	public VoidNoneParameter onClickLeaveGame;

	public override IEnumerator Init ()
	{
		image_Mask.gameObject.SetActive(false);
		image_SettingWindow.gameObject.SetActive(false);
		image_LeaveWindow.gameObject.SetActive(false);
		group_Main.gameObject.SetActive(true);
		backEvent = OnClickBack;
		yield return 0;
	}

	public void OnClick1P()
	{
		if(onClick1P != null)
			onClick1P();
	}
	
	public void OnClick2P()
	{
		if(onClick2P != null)
			onClick2P();
	}

	public void OnClickShop()
	{
		if(onClickShop != null)
			onClickShop();
	}

	public void OnClickRate()
	{
		if(onClickRate != null)
			onClickRate();
	}

	public void OnClickMail()
	{
		if(onClickMail != null)
			onClickMail();
	}

	public void OnClickSetting()
	{
		currentState = ViewState.SettingWindow;
		image_Mask.gameObject.SetActive(true);
		image_Mask.alpha = 0f;
		image_Mask.DOFade(1f, 0.3f);		
		image_SettingWindow.gameObject.SetActive(true);
		image_SettingWindow.localScale = Vector3.zero;
		image_SettingWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
	}

	public void OnClickExitSetting()
	{
		currentState = ViewState.Main;
		image_Mask.DOFade(0f, 0.3f);
		image_SettingWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(
			delegate(){
			image_Mask.gameObject.SetActive(false);
			image_SettingWindow.gameObject.SetActive(false);
		}
		);
	}

	public void OnClickExitLeaveWindow()
	{
		currentState = ViewState.Main;
		image_Mask.DOFade(0f, 0.3f);
		image_LeaveWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(
			delegate(){
			image_Mask.gameObject.SetActive(false);
			image_LeaveWindow.gameObject.SetActive(false);
		}
		);
	}

	public void OnClickLeaveGame()
	{
		if(onClickLeaveGame != null)
			onClickLeaveGame();
	}

	void OnClickBack()
	{
		switch(currentState)
		{
		case ViewState.Main:
			ShowLeaveWindow();
			break;
		case ViewState.SettingWindow:
			OnClickExitSetting();
			break;
		case ViewState.LeaveWindow:
			OnClickExitLeaveWindow();
			break;
		}
	}

	void ShowLeaveWindow()
	{
		currentState = ViewState.LeaveWindow;
		image_Mask.gameObject.SetActive(true);
		image_Mask.alpha = 0f;
		image_Mask.DOFade(1f, 0.3f);	
		image_LeaveWindow.gameObject.SetActive(true);
		image_LeaveWindow.localScale = Vector3.zero;
		image_LeaveWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
	}

	protected override IEnumerator HideUIAnimation ()
	{
		currentState = ViewState.Close;
		group_Main.anchoredPosition = Vector2.zero;
		yield return group_Main.DOAnchorPos(hideLeft, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		base.HideUI(false);
	}

	protected override IEnumerator ShowUIAnimation ()
	{
		currentState = ViewState.Close;
		group_Main.anchoredPosition = hideLeft;
		group_Main.gameObject.SetActive(true);
		image_Mask.gameObject.SetActive(false);
		image_SettingWindow.gameObject.SetActive(false);
		image_LeaveWindow.gameObject.SetActive(false);

		yield return group_Main.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();

		currentState = ViewState.Main;
	}
}
