using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class MainPageView : AbstractView {
	enum ViewState{Main, SettingWindow, Close}
	ViewState currentState;
	public RectTransform group_Main;
    public Image image_Mask;
    public RectTransform image_SettingWindow;
	public VoidNoneParameter onClick1P;
	public VoidNoneParameter onClick2P;
	public VoidNoneParameter onClickRate;
	public VoidNoneParameter onClickMail;
	public VoidNoneParameter onClickShop;

	public override IEnumerator Init ()
	{
		image_Mask.gameObject.SetActive(false);
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
        image_Mask.color = Color.clear;
        image_Mask.DOColor(Color.black * 0.5f, 0.3f);
        image_SettingWindow.anchoredPosition = hideDown;
        image_SettingWindow.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
	}

	public void OnClickExitSetting()
	{
		currentState = ViewState.Main;
        image_SettingWindow.DOAnchorPos(hideDown, 0.3f).SetEase(Ease.InQuad);
        image_Mask.DOColor(Color.clear, 0.3f).OnComplete(
            delegate () {
                image_Mask.gameObject.SetActive(false);
            }
        );
	}
    
	void OnClickBack()
	{
		switch(currentState)
		{
		case ViewState.SettingWindow:
			OnClickExitSetting();
			break;
		}
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

		yield return group_Main.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();

		currentState = ViewState.Main;
	}
}
