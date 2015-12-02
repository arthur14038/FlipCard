using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class MainPageView : AbstractView {
	enum ViewState{Main, OnePlayer, TwoPlayer, SettingWindow, Shop, LeaveWindow}
	ViewState currentState;
	public VoidCardArrayLevel onClickPlay;
	public CanvasGroup group_Main;
	public CanvasGroup group_1P;
	public CanvasGroup group_2P;
	public CanvasGroup image_Mask;
	public RectTransform image_SettingWindow;
	public RectTransform image_LeaveWindow;
	public Image[] levelMask;

	public override IEnumerator Init ()
	{
		yield return 0;
	}

	public IEnumerator Init(int view)
	{
		image_Mask.gameObject.SetActive(false);
		image_SettingWindow.gameObject.SetActive(false);
		image_LeaveWindow.gameObject.SetActive(false);
		for(int i = 0 ; i < levelMask.Length ; ++i)
		{
			if(i > PlayerPrefsManager.OnePlayerProgress)
			{
				levelMask[i].gameObject.SetActive(true);
			}else
			{
				levelMask[i].gameObject.SetActive(false);
			}
		}
		backEvent = OnClickBack;
		currentState = (ViewState)view;
		switch(view)
		{
		case 0:
			group_Main.gameObject.SetActive(true);
			group_1P.gameObject.SetActive(false);
			group_2P.gameObject.SetActive(false);
			break;
		case 1:
			group_Main.gameObject.SetActive(false);
			group_1P.gameObject.SetActive(true);
			group_2P.gameObject.SetActive(false);
			break;
		case 2:
			group_Main.gameObject.SetActive(false);
			group_1P.gameObject.SetActive(false);
			group_2P.gameObject.SetActive(true);
			break;
		}
		yield return 0;
	}

	public void OnClickPlay(int level)
	{
		if(onClickPlay != null)
			onClickPlay((CardArrayLevel)level);
	}

	public void OnClick1P()
	{
		currentState = ViewState.OnePlayer;
		group_Main.gameObject.SetActive(false);
		group_1P.gameObject.SetActive(true);
	}
	
	public void OnClick2P()
	{
	}

	public void OnClickShop()
	{
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

	public void OnClickBackMain()
	{
		currentState = ViewState.Main;
		group_Main.gameObject.SetActive(true);
		group_1P.gameObject.SetActive(false);
		group_2P.gameObject.SetActive(false);
	}

	public void OnClickLeaveGame()
	{
	}

	void OnClickBack()
	{
		switch(currentState)
		{
		case ViewState.Main:
			currentState = ViewState.LeaveWindow;
			image_Mask.gameObject.SetActive(true);
			image_Mask.alpha = 0f;
			image_Mask.DOFade(1f, 0.3f);	
			image_LeaveWindow.gameObject.SetActive(true);
			image_LeaveWindow.localScale = Vector3.zero;
			image_LeaveWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
			break;
		case ViewState.OnePlayer:
			OnClickBackMain();
			break;
		case ViewState.TwoPlayer:
			OnClickBackMain();
			break;
		case ViewState.SettingWindow:
			OnClickExitSetting();
			break;
		case ViewState.LeaveWindow:
			OnClickExitLeaveWindow();
			break;
		case ViewState.Shop:
			OnClickBackMain();
			break;
		}
	}

	protected override IEnumerator HideUIAnimation ()
	{
		yield return 0;
	}

	protected override IEnumerator ShowUIAnimation ()
	{
		yield return 0;
	}
}
