using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class ShopView : AbstractView
{
	enum ShopGroup {Theme, Shop, None}
	ShopGroup currentGroup = ShopGroup.None;
	enum MsgWindowState {Close, ConfirmBuy, NotEnoughMoni, ThemeInfo, GetMoni}
	MsgWindowState currentMsgWindowState;
	public VoidNoneParameter onClickBack;
	public VoidString onClickThemeInfo;
	public VoidString onClickEquipTheme;
	public VoidThemeInfo onClickThemePrice;
	public VoidTwoString onClickEquipCard;
	public VoidNoneParameter onClickConfirmBuyTheme;
	public VoidNoneParameter onClickMoniBoard;
	public Text text_CurrentMoni;
	public RectTransform group_Shop;
	public RectTransform content_ThemeUI;
	public RectTransform group_Theme;
	public Image image_PopUpMask;
	public RectTransform group_MsgWindow;
	public Text text_MsgTitle;
	public Text text_MsgContent;
	public Toggle[] swithToggle;
	public Image group_LoadingWindow;
	public RectTransform group_ThemeInfoWindow;
	public Text text_ThemeName;
	public Text text_ThemeInfo;
	public RectTransform image_LoadingWindowBG;
	public Image image_Green;
	public Image image_Orange;
	public Image image_Pink;
	public Image image_Blue;
	public Image image_Move;
	public Text text_Loading;
	public RectTransform group_Loading;
	public RectTransform group_ResultMsg;
	public Text text_ResultMsg;
	public List<ThemePackUI> themePackUIList = new List<ThemePackUI>();
	public GameObject group_AwardAd;
	public GameObject group_ThemeBuyingInfo;
	public ScrollRect scrollView;
	public GameObject group_SoldOut;
	public GameObject button_Cancel;
	public GameObject button_Yes;
	public GameObject button_OK;
	Coroutine loadingAnimation;
	Vector3 rotateAngle = new Vector3(0f, 0f, 120f);

	public override IEnumerator Init()
	{
		escapeEvent = OnClickBack;
		InventoryManager.Instance.updateCurrency += UpdateMoniCount;
		UpdateMoniCount();

		List<ThemeInfo> themeInfoList = InventoryManager.Instance.GetAllThemeInfo();

		for(int i = 0 ; i < themeInfoList.Count ; ++i)
		{
			themePackUIList[i].Init(themeInfoList[i], OnClickEquipTheme, OnClickEquipCard, OnClickThemePrice, OnClickThemeInfo);
		}

		image_PopUpMask.gameObject.SetActive(false);
		group_LoadingWindow.gameObject.SetActive(false);
		currentMsgWindowState = MsgWindowState.Close;
		SetCurrentGroup(ShopGroup.Theme);
		yield return null;
	}

	public void OnClickBack()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		if(onClickBack != null)
			onClickBack();
	}

	public void OnClickCancelMsg()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		StartCoroutine(CloseMsgWindow());
	}

	public void OnClickYesMsg()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		switch(currentMsgWindowState)
		{
			case MsgWindowState.ConfirmBuy:
				if(onClickConfirmBuyTheme != null)
					onClickConfirmBuyTheme();
				break;
		}
		StartCoroutine(CloseMsgWindow());
	}

	public void OnClickMoniBoard()
	{
		if(onClickMoniBoard != null)
			onClickMoniBoard();
    }
	
	public void ToggleGroup(int groupIndex)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(!swithToggle[groupIndex].isOn)
			swithToggle[groupIndex].isOn = true;

		SetCurrentGroup((ShopGroup)groupIndex);
	}

	public void ShowGetMoni(int amount)
	{
		currentMsgWindowState = MsgWindowState.GetMoni;
		ShowMsgWindow("Special Award!", string.Format("You get {0} Moni by watching the ad!", amount));
		button_OK.SetActive(true);
		button_Yes.SetActive(false);
		button_Cancel.SetActive(false);
	}

	public void ShowMoniNotEnough()
	{
		currentMsgWindowState = MsgWindowState.NotEnoughMoni;
		ShowMsgWindow("Your Moni is not enough", "You don't have enough \nMoni to buy this theme.");
		button_OK.SetActive(true);
		button_Yes.SetActive(false);
		button_Cancel.SetActive(false);
	}

	public void ShowConfirmBuy(int cost)
	{
		currentMsgWindowState = MsgWindowState.ConfirmBuy;
		string content = string.Format("Buying this theme will cost \n{0} Moni.\nAre you sure?", cost);
		ShowMsgWindow("Confirm Buy", content);
		button_OK.SetActive(false);
		button_Yes.SetActive(true);
		button_Cancel.SetActive(true);
	}

	public void ShowThemeInfo(string themeName, string themeContent)
	{
		currentMsgWindowState = MsgWindowState.ThemeInfo;
		ShowMsgWindow(themeName, themeContent);
	}

	public void ShowBuyMsg(string message)
	{
		StartCoroutine(LoadingEndEffect(message));
	}

	public void ShowLoadingWindow()
	{
		group_Loading.gameObject.SetActive(true);
		group_ResultMsg.gameObject.SetActive(false);
		group_LoadingWindow.gameObject.SetActive(true);
		group_LoadingWindow.color = Color.clear;
		group_LoadingWindow.DOColor(Color.black * 0.7f, 0.3f);
		image_LoadingWindowBG.localScale = Vector3.zero;
		image_LoadingWindowBG.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
		DoLoadingAnimation();
	}

	public void CloseLoadingWindow()
	{
		image_LoadingWindowBG.DOScale(0f, 0.3f).SetEase(Ease.InBack);
		group_LoadingWindow.DOFade(0f, 0.3f).OnComplete(
			delegate () {
				group_LoadingWindow.gameObject.SetActive(false);
			}
		);
		if(loadingAnimation != null)
		{
			StopCoroutine(loadingAnimation);
			loadingAnimation = null;
		}
	}

	public void UpdateThemePackList()
	{
		foreach(ThemePackUI themePackUI in themePackUIList)
		{
			themePackUI.CheckUIState();
		}
		SetCurrentGroup(currentGroup, false);
	}

	void UpdateMoniCount()
	{
		text_CurrentMoni.text = PlayerPrefsManager.MoniCount.ToString();
	}

	void ShowMsgWindow(string title, string content, int group = 0)
	{
		switch(currentMsgWindowState)
		{
			case MsgWindowState.ThemeInfo:
				text_ThemeName.text = title;
				text_ThemeInfo.text = content;
				group_MsgWindow.gameObject.SetActive(false);
				group_ThemeInfoWindow.gameObject.SetActive(true);
				group_ThemeInfoWindow.localScale = Vector3.zero;
				group_ThemeInfoWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
				break;
			case MsgWindowState.GetMoni:
			case MsgWindowState.ConfirmBuy:
			case MsgWindowState.NotEnoughMoni:
				text_MsgTitle.text = title;
				text_MsgContent.text = content;
				group_ThemeInfoWindow.gameObject.SetActive(false);
				group_MsgWindow.gameObject.SetActive(true);
				group_MsgWindow.localScale = Vector3.zero;
				group_MsgWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
				break;
		}

		image_PopUpMask.gameObject.SetActive(true);
		image_PopUpMask.color = Color.clear;
		image_PopUpMask.DOColor(Color.black * 0.7f, 0.3f);
	}

	IEnumerator CloseMsgWindow()
	{
		switch(currentMsgWindowState)
		{
			case MsgWindowState.ThemeInfo:
				group_ThemeInfoWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack);
				break;
			case MsgWindowState.ConfirmBuy:
				GoogleAnalyticsManager.LogEvent(GoogleAnalyticsManager.EventCategory.BuyTheme,
					GoogleAnalyticsManager.EventAction.CancelBuyTheme);

				group_MsgWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack);
				break;
			case MsgWindowState.GetMoni:
			case MsgWindowState.NotEnoughMoni:
				group_MsgWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack);
				break;
		}
		yield return image_PopUpMask.DOFade(0f, 0.3f).WaitForCompletion();
		image_PopUpMask.gameObject.SetActive(false);
		currentMsgWindowState = MsgWindowState.Close;
	}

	void DoLoadingAnimation()
	{
		loadingAnimation = StartCoroutine(LoadingAnimation());
	}

	IEnumerator LoadingAnimation()
	{
		image_Move.rectTransform.anchoredPosition = image_Green.rectTransform.anchoredPosition;
		image_Move.color = image_Green.color;

		text_Loading.DOFade(0.6f, 0.5f).OnComplete(delegate () {
			text_Loading.DOFade(1f, 0.5f);
		}
		);

		image_Move.rectTransform.DOAnchorPos(image_Orange.rectTransform.anchoredPosition, 0.25f);
		image_Move.DOColor(image_Orange.color, 0.25f);
		yield return image_Green.rectTransform.DOScaleX(0f, 3 / 24f).WaitForCompletion();
		yield return image_Green.rectTransform.DOScaleX(1.3f, 2 / 24f).WaitForCompletion();
		yield return image_Green.rectTransform.DOScaleX(1f, 1 / 24f).WaitForCompletion();

		image_Move.rectTransform.DOAnchorPos(image_Blue.rectTransform.anchoredPosition, 0.25f);
		image_Move.DOColor(image_Blue.color, 0.25f).WaitForCompletion();
		yield return image_Orange.rectTransform.DOScaleX(0f, 3 / 24f).WaitForCompletion();
		yield return image_Orange.rectTransform.DOScaleX(1.3f, 2 / 24f).WaitForCompletion();
		yield return image_Orange.rectTransform.DOScaleX(1f, 1 / 24f).WaitForCompletion();

		image_Move.rectTransform.DOAnchorPos(image_Pink.rectTransform.anchoredPosition, 0.25f);
		image_Move.DOColor(image_Pink.color, 0.25f).WaitForCompletion();
		yield return image_Blue.rectTransform.DOScaleX(0f, 3 / 24f).WaitForCompletion();
		yield return image_Blue.rectTransform.DOScaleX(1.3f, 2 / 24f).WaitForCompletion();
		yield return image_Blue.rectTransform.DOScaleX(1f, 1 / 24f).WaitForCompletion();

		image_Move.rectTransform.DOAnchorPos(image_Green.rectTransform.anchoredPosition, 0.25f);
		image_Move.DOColor(image_Green.color, 0.25f).WaitForCompletion();
		image_Pink.rectTransform.DOScale(1.3f, 1 / 24f).OnComplete(
			delegate () {
				image_Pink.rectTransform.DOScale(1f, 3 / 24f);
			}
		);
		image_Pink.rectTransform.DORotate(-rotateAngle, 3 / 24f).OnComplete(delegate () {
			image_Pink.rectTransform.DORotate(Vector3.zero, 7 / 24f);
		}
		);
		yield return new WaitForSeconds(0.25f);

		DoLoadingAnimation();
	}

	IEnumerator LoadingEndEffect(string message)
	{
		if(loadingAnimation != null)
		{
			StopCoroutine(loadingAnimation);
			loadingAnimation = null;
		}
		text_ResultMsg.text = message;
		yield return group_Loading.DOScale(0f, 0.3f).WaitForCompletion();
		group_Loading.gameObject.SetActive(false);
		group_ResultMsg.localScale = Vector3.zero;
		group_ResultMsg.gameObject.SetActive(true);
		yield return group_ResultMsg.DOScale(1f, 0.3f).SetEase(Ease.OutBack).WaitForCompletion();
	}

	IEnumerator WaveEffect(RectTransform waveItem, float duration)
	{
		waveItem.rotation = Quaternion.Euler(Vector3.zero);
		yield return waveItem.DORotate(Vector3.back * (10f + Random.Range(-2f, 2f)), duration).SetEase(Ease.OutBounce).WaitForCompletion();
		float noise = Random.Range(0f, 0.125f);
		yield return waveItem.DORotate(Vector3.forward * 7.5f, 0.125f + noise).SetEase(Ease.OutQuad).WaitForCompletion();
		noise = Random.Range(0f, 0.125f);
		yield return waveItem.DORotate(Vector3.back * 5.0f, 0.125f + noise).SetEase(Ease.OutQuad).WaitForCompletion();
		noise = Random.Range(0f, 0.125f);
		yield return waveItem.DORotate(Vector3.forward * 2.5f, 0.125f + noise).SetEase(Ease.OutQuad).WaitForCompletion();
		noise = Random.Range(0f, 0.125f);
		yield return waveItem.DORotate(Vector3.zero, 0.125f + noise).SetEase(Ease.OutQuad).WaitForCompletion();
	}

	void SetCurrentGroup(ShopGroup value, bool repositionScrollView = true)
	{
		currentGroup = value;

		if(PlayerPrefsManager.CanShowAwardAd && Advertisement.IsReady("rewardedVideo"))
			group_AwardAd.gameObject.SetActive(true);
		else
			group_AwardAd.gameObject.SetActive(false);

		switch(currentGroup)
		{
			case ShopGroup.Theme:
				if(repositionScrollView)
					scrollView.normalizedPosition = Vector2.zero;
				foreach(ThemePackUI themePackUI in themePackUIList)
				{
					if(themePackUI.IsInBag)
						themePackUI.gameObject.SetActive(true);
					else
						themePackUI.gameObject.SetActive(false);
				}
				group_ThemeBuyingInfo.gameObject.SetActive(true);
				group_Theme.gameObject.SetActive(true);
				group_SoldOut.SetActive(false);
                break;
			case ShopGroup.Shop:
				if(repositionScrollView)
					scrollView.normalizedPosition = Vector2.zero;
				int availableCount = 0;
				foreach(ThemePackUI themePackUI in themePackUIList)
				{
					if(themePackUI.IsInBag)
					{
						themePackUI.gameObject.SetActive(false);
					}else
					{
						++availableCount;
						themePackUI.gameObject.SetActive(true);
					}
				}
				if(availableCount > 0)
				{
					group_SoldOut.SetActive(false);
					group_Theme.gameObject.SetActive(true);
				} else
				{
					group_SoldOut.SetActive(true);
					group_Theme.gameObject.SetActive(false);
				}
				group_ThemeBuyingInfo.gameObject.SetActive(false);
				break;
		}
	}

	void OnClickEquipTheme(string themeItemId)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickEquipTheme != null)
			onClickEquipTheme(themeItemId);
	}

	void OnClickEquipCard(string cardFaceItemId, string cardBackItemId)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickEquipCard != null)
			onClickEquipCard(cardBackItemId, cardFaceItemId);
	}

	void OnClickThemePrice(ThemeInfo themeInfo)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickThemePrice != null)
			onClickThemePrice(themeInfo);
    }

	void OnClickThemeInfo(string themeItemId)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickThemeInfo != null)
			onClickThemeInfo(themeItemId);
	}

	protected override IEnumerator HideUIAnimation()
	{
		group_Shop.anchoredPosition = Vector2.zero;
		yield return group_Shop.DOAnchorPos(hideRight, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		base.HideUI(false);
		hideCoroutine = null;
	}

	protected override IEnumerator ShowUIAnimation()
	{
		switch(currentGroup)
		{
			case ShopGroup.Theme:
			case ShopGroup.Shop:
				foreach(ThemePackUI themePackUI in themePackUIList)
					if(themePackUI.gameObject.activeInHierarchy)
						StartCoroutine(themePackUI.EnterEffect(0.5f));
				break;
		}
		group_Shop.gameObject.SetActive(true);
		group_Shop.anchoredPosition = hideRight;
		yield return group_Shop.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic).WaitForCompletion();
		showCoroutine = null;
	}	

	void OnDestroy()
	{
		if(InventoryManager.Instance != null)
			InventoryManager.Instance.updateCurrency -= UpdateMoniCount;
	}
}
