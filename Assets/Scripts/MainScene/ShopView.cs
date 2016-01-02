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
	public VoidNoneParameter onClickBack;
	public Text text_CurrentMoni;
	public RectTransform group_Shop;
	public RectTransform content_ThemeUI;
	public RectTransform group_Theme;
	public RectTransform group_Moni;
	List<ThemePackUI> themePackList = new List<ThemePackUI>();

	public override IEnumerator Init()
	{
		SetCurrentGroup(ShopGroup.Theme);
		text_CurrentMoni.text = StoreInventory.GetItemBalance(FlipCardStoreAsset.MONI_ITEM_ID).ToString();
		yield return null;
	}

	public void OnClickBack()
	{
		AudioManager.Instance.PlayOneShot("Button_Click2");
		if(onClickBack != null)
			onClickBack();
	}

	public void BuyMoniPack(int tier)
	{
		Debug.LogFormat("BuyMoniPack Tier {0}", tier);
	}

	public void ToggleGroup(int groupIndex)
	{
		AudioManager.Instance.PlayOneShot("Button_Click");
		SetCurrentGroup((ShopGroup)groupIndex);
	}

	void SetCurrentGroup(ShopGroup value)
	{
		currentGroup = value;
		group_Theme.gameObject.SetActive(false);
		group_Moni.gameObject.SetActive(false);
		switch(currentGroup)
		{
			case ShopGroup.Theme:
				group_Theme.gameObject.SetActive(true);
				break;
			case ShopGroup.Shop:
				group_Theme.gameObject.SetActive(true);
				break;
			case ShopGroup.Moni:
				group_Moni.gameObject.SetActive(true);
				break;
		}
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
}
