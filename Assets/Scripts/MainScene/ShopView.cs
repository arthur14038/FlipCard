using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using Soomla.Store;

public class ShopView : AbstractView
{
	enum ShopGroup {Theme, Shop, Moni, None}
	ShopGroup currentGroup = ShopGroup.None;
	enum MsgWindowState {Close, ConfirmBuy, NotEnoughMoni}
	MsgWindowState currentMsgWindowState;
	public VoidNoneParameter onClickBack;
	public VoidString onClickEquipTheme;
	public VoidThemePack onClickThemePrice;
	public VoidTwoString onClickEquipCard;
	public VoidInt onClickBuyMoniPack;
	public VoidNoneParameter onClickConfirmBuyTheme;
	public Text text_CurrentMoni;
	public RectTransform group_Shop;
	public RectTransform content_ThemeUI;
	public RectTransform group_Theme;
	public RectTransform group_Moni;
	public Image image_PopUpMask;
	public RectTransform image_MsgWindow;
	public Text text_MsgTitle;
	public Text text_MsgContent;
	public Toggle[] swithToggle;
	public Image group_LoadingWindow;
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
	Coroutine loadingAnimation;
	Vector3 rotateAngle = new Vector3(0f, 0f, 120f);

	public override IEnumerator Init()
	{
		InventoryManager.Instance.updateCurrency += UpdateMoniCount;
		UpdateMoniCount();

		List<ThemePack> themePackList = InventoryManager.Instance.GetAllThemePack();

		for(int i = 0 ; i < themePackList.Count ; ++i)
		{
			themePackUIList[i].Init(themePackList[i], OnClickEquipTheme, OnClickEquipCard, OnClickThemePrice);
		}
		//for(int i = 0 ; i < themePackList.Count ; ++i)
		//{
		//	GameObject tmp = Instantiate(themePackUIPrefab) as GameObject;
		//	tmp.transform.SetParent(content_ThemeUI);
		//	tmp.transform.localScale = Vector3.one;
		//	tmp.name = themePackUIPrefab.name + i.ToString();
		//	ThemePackUI themePackUI = tmp.GetComponent<ThemePackUI>();
		//	themePackUI.Init(themePackList[i], OnClickEquipTheme, OnClickEquipCard, OnClickThemePrice);
		//	themePackUIDictionary.Add(i, themePackUI);
		//}
		//themePackUIPrefab = null;

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
			case MsgWindowState.NotEnoughMoni:
				swithToggle[(int)ShopGroup.Moni].isOn = true;
				SetCurrentGroup(ShopGroup.Moni);
				break;
		}
		StartCoroutine(CloseMsgWindow());
	}
	
	public void BuyMoniPack(int tier)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickBuyMoniPack != null)
			onClickBuyMoniPack(tier);
	}

	public void ToggleGroup(int groupIndex)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		SetCurrentGroup((ShopGroup)groupIndex);
	}

	public void ShowMoniNotEnough()
	{
		currentMsgWindowState = MsgWindowState.NotEnoughMoni;
		ShowMsgWindow("Your Moni is not enough", "You don't have enough \nMoni to buy this theme.\nDo you want to buy some Moni?");
	}

	public void ShowConfirmBuy(VirtualGood good)
	{
		currentMsgWindowState = MsgWindowState.ConfirmBuy;
		string content = string.Format("Buying this theme will cost \n{0} Moni.\nAre you sure?", good.PurchaseType.GetPrice());
		ShowMsgWindow("Confirm Buy", content);
	}

	public void ShowBuyMsg(bool success)
	{
		if(loadingAnimation != null)
		{
			StopCoroutine(loadingAnimation);
			loadingAnimation = null;
		}
		string content = "";
		if(success)
		{
			content = "BUYING SUCCESS!";
		}else
		{
			content = "SOME ERROR OCCUR";
		}
		text_ResultMsg.text = content;
		group_Loading.gameObject.SetActive(false);
		group_ResultMsg.gameObject.SetActive(true);
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
		SetCurrentGroup(currentGroup);
	}

	void UpdateMoniCount()
	{
		text_CurrentMoni.text = StoreInventory.GetItemBalance(FlipCardStoreAsset.MONI_ITEM_ID).ToString();
	}

	void ShowMsgWindow(string title, string content)
	{
		text_MsgTitle.text = title;
		text_MsgContent.text = content;

		image_PopUpMask.gameObject.SetActive(true);
		image_PopUpMask.color = Color.clear;
		image_PopUpMask.DOColor(Color.black * 0.7f, 0.3f);
		image_MsgWindow.localScale = Vector3.zero;
		image_MsgWindow.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
	}

	IEnumerator CloseMsgWindow()
	{
		image_MsgWindow.DOScale(0f, 0.3f).SetEase(Ease.InBack);
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

	void SetCurrentGroup(ShopGroup value)
	{
		currentGroup = value;
		switch(currentGroup)
		{
			case ShopGroup.Theme:
				foreach(ThemePackUI themePackUI in themePackUIList)
				{
					if(themePackUI.IsInBag)
						themePackUI.gameObject.SetActive(true);
					else
						themePackUI.gameObject.SetActive(false);
				}
				group_Moni.gameObject.SetActive(false);
				group_Theme.gameObject.SetActive(true);
				break;
			case ShopGroup.Shop:
				foreach(ThemePackUI themePackUI in themePackUIList)
				{
					if(themePackUI.IsInBag)
						themePackUI.gameObject.SetActive(false);
					else
						themePackUI.gameObject.SetActive(true);
				}
				group_Moni.gameObject.SetActive(false);
				group_Theme.gameObject.SetActive(true);
				break;
			case ShopGroup.Moni:
				group_Theme.gameObject.SetActive(false);
				group_Moni.gameObject.SetActive(true);
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
			onClickEquipCard(cardFaceItemId, cardBackItemId);
	}

	void OnClickThemePrice(ThemePack themePack)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		if(onClickThemePrice != null)
			onClickThemePrice(themePack);
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
